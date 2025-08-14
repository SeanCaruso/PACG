
using PACG.SharedAPI;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PACG.Gameplay
{
    public class ActionStagingManager
    {
        // Dependency Injections
        private CardManager _cards;
        private ContextManager _contexts;
        private GameFlowManager _gameFlow;     // Commit -> Continue processing
        private GameServices _gameServices;    // Passed in to IResolvable.CreateProcessor

        // Other members
        private readonly Dictionary<PlayerCharacter, List<IStagedAction>> _pcsStagedActions = new();
        private readonly Dictionary<CardInstance, CardLocation> _originalCardLocs = new();

        private bool _hasExploreStaged;
        public bool HasExploreStaged => _hasExploreStaged;

        public IReadOnlyList<CardInstance> StagedCards => _originalCardLocs.Keys.ToList();
        public bool CardStaged(CardInstance card) => _originalCardLocs.Keys.Contains(card);

        public void Initialize(GameServices gameServices)
        {
            _cards = gameServices.Cards;
            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
            _gameServices = gameServices;
        }

        public void StageAction(IStagedAction action)
        {
            var pcActions = _pcsStagedActions.GetValueOrDefault(action.Card.Owner, new());

            if (pcActions.Contains(action))
            {
                Debug.LogWarning($"{action.Card.Data.cardName}.{action} staged multiple times!");
                return;
            }

            if (_contexts.CheckContext?.CanStageAction(action) == false)
            {
                Debug.LogWarning($"{action.Card.Data.cardName}.{action} can't be staged! How did we get this far?");
                return;
            }

            _hasExploreStaged = action is ExtraExploreAction;

            // If this is the first staged action for this card, store where it originally came from.
            _originalCardLocs.TryAdd(action.Card, action.Card.CurrentLocation);

            // We need to handle this here so that damage resolvables behave with hand size.
            _cards.MoveCard(action.Card, action.ActionType);

            // Perform all required staging logic.
            action.OnStage();
            pcActions.Add(action);
            _pcsStagedActions[action.Card.Owner] = pcActions;

            // If we're attempting a check, add the action's card type if needed.
            _contexts.CheckContext?.StageCardTypeIfNeeded(action);

            // Send the UI event to update Cancel/Commit buttons.
            UpdateActionButtonState();
        }

        public void Cancel()
        {
            // Always do the standard undo logic
            PlayerCharacter pc = _contexts.TurnContext.Character;
            foreach (var action in _pcsStagedActions[pc])
            {
                action.OnUndo();
            }

            foreach ((var card, var location) in _originalCardLocs)
            {
                _cards.MoveCard(card, location);
            }
            GameEvents.RaiseCardLocationsChanged(_originalCardLocs.Keys.ToList());

            _hasExploreStaged = false;
            _originalCardLocs.Clear();
            _pcsStagedActions[pc].Clear();
            _contexts.CheckContext?.ClearStagedTypes();

            // Additional step for phase-level cancels
            if (_contexts.CurrentResolvable?.CancelAbortsPhase == true)
            {
                GameEvents.SetStatusText("");
                _gameFlow.AbortPhase();
                _contexts.EndResolvable();
                _pcsStagedActions.Clear(); // Clear ALL PCs' actions when aborting phase
            }

            UpdateActionButtonState();
        }

        public void UpdateActionButtonState()
        {
            // TODO: Update this for the displayed PC. Use Turn PC until then.
            var pc = _contexts.TurnContext.Character;
            var stagedActions = _pcsStagedActions.GetValueOrDefault(pc) ?? (_pcsStagedActions[pc] = new());

            bool canCommit = stagedActions.Count > 0 && (_contexts.CurrentResolvable?.CanCommit(stagedActions) ?? true); // We have actions but no resolvable? We can commit!
            bool canSkip = stagedActions.Count == 0 && (_contexts.CurrentResolvable?.CanCommit(stagedActions) ?? false); // We don't have any actions and no resolvable to skip, so false!

            StagedActionsState state = new(
                canCancel: stagedActions.Count > 0 || _contexts.CurrentResolvable?.CancelAbortsPhase == true,
                canCommit: canCommit && !_hasExploreStaged,
                canSkip: canSkip && !_hasExploreStaged,
                isExploreEnabled: _contexts.TurnContext.CanExplore || _hasExploreStaged);
            GameEvents.RaiseStagedActionsStateChanged(state);
        }

        public void Commit()
        {
            // Clear status text first.
            GameEvents.SetStatusText("");

            foreach (var action in _pcsStagedActions.Values.SelectMany(list => list))
            {
                action.Commit(_contexts.CheckContext);
            }
            _hasExploreStaged = false;
            _originalCardLocs.Clear();
            _pcsStagedActions.Clear();
            _cards.RestoreRevealedCardsToHand();

            // If we have a resolvable, the fact that we committed means that it's been resolved.
            if (_contexts.CurrentResolvable != null)
            {
                // If it requires a processor, kick off a new phase immediately.
                var processor = _contexts.CurrentResolvable.CreateProcessor(_gameServices);
                if (processor != null)
                {
                    Debug.Log($"[{GetType().Name}] {_contexts.CurrentResolvable} created {processor}");
                    _gameFlow.StartPhase(processor, _contexts.CurrentResolvable.GetType().Name);
                }
                else
                {
                    Debug.Log($"[{GetType().Name}] {_contexts.CurrentResolvable} didn't queue a processor.");
                }
                _contexts.EndResolvable();
            }

            UpdateActionButtonState();

            // We're done committing actions. Tell the GameFlowManager to continue.
            _gameFlow.Process();
        }
    }
}
