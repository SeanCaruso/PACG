using PACG.SharedAPI;
using System.Collections.Generic;
using System.Linq;
using PACG.Core;
using UnityEngine;

namespace PACG.Gameplay
{
    public class ActionStagingManager
    {
        // Dependency Injections
        private CardManager _cards;
        private ContextManager _contexts;
        private GameFlowManager _gameFlow; // Commit -> Continue processing
        private GameServices _gameServices; // Passed in to IResolvable.CreateProcessor

        // Other members
        private readonly Dictionary<PlayerCharacter, List<IStagedAction>> _pcsStagedActions = new();
        private readonly Dictionary<CardInstance, CardLocation> _originalCardLocs = new();

        private bool HasExploreStaged { get; set; }

        public IReadOnlyList<IStagedAction> StagedActionsFor(PlayerCharacter pc) =>
            _pcsStagedActions.GetValueOrDefault(pc, new List<IStagedAction>());

        public IReadOnlyList<IStagedAction> StagedActions => _pcsStagedActions.Values.SelectMany(list => list).ToList();
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
            var pcActions = _pcsStagedActions.GetValueOrDefault(
                action.Card.Owner, new List<IStagedAction>()
            );

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

            HasExploreStaged = action is ExploreAction;

            // If this is the first staged action for this card, store where it originally came from.
            _originalCardLocs.TryAdd(action.Card, action.Card.CurrentLocation);

            // We need to handle this here so that damage resolvables behave with hand size.
            _cards.MoveCard(action.Card, action.ActionType);

            // Perform all required staging logic.
            pcActions.Add(action);
            _pcsStagedActions[action.Card.Owner] = pcActions;

            // If we're attempting a check, add the action's card type if needed.
            _contexts.CheckContext?.StageCardTypeIfNeeded(action);

            UpdateGameStatePreview();
            // Send the UI event to update Cancel/Commit buttons.
            UpdateActionButtons();
        }

        public void Cancel()
        {
            // Always do the standard undo logic
            var pc = _contexts.TurnContext.Character;

            foreach (var (card, location) in _originalCardLocs)
            {
                _cards.MoveCard(card, location);
            }

            GameEvents.RaiseCardLocationsChanged(_originalCardLocs.Keys.ToList());

            HasExploreStaged = false;
            _originalCardLocs.Clear();
            if (_pcsStagedActions.TryGetValue(pc, out var pcActions))
                pcActions.Clear();

            // Additional step for phase-level cancels
            if (_contexts.CurrentResolvable?.CancelAbortsPhase == true)
            {
                GameEvents.SetStatusText("");
                _gameFlow.AbortPhase();
                _contexts.EndResolvable();
                _pcsStagedActions.Clear(); // Clear ALL PCs' actions when aborting phase
            }

            UpdateGameStatePreview();
            UpdateActionButtons();
        }

        public void UpdateGameStatePreview()
        {
            _contexts.CheckContext?.UpdatePreviewState(StagedActions);
        }

        public void UpdateActionButtons()
        {
            // Send an event to update the state of the action buttons (Cancel, Commit, Skip).
            // TODO: Update this for the displayed PC. Use Turn PC until then.
            var pc = _contexts.TurnContext?.Character;
            var stagedActions = pc != null
                ? _pcsStagedActions.GetValueOrDefault(pc, new List<IStagedAction>())
                : new List<IStagedAction>();

            var state = _contexts.CurrentResolvable?.GetUIState(stagedActions) ?? GetDefaultUiState(stagedActions);
            GameEvents.RaiseStagedActionsStateChanged(state);
        }

        private StagedActionsState GetDefaultUiState(List<IStagedAction> actions)
        {
            return new StagedActionsState(
                canCommit: actions.Any() && !HasExploreStaged,
                canSkip: false,
                canCancel: actions.Any(),
                isExploreEnabled: _contexts.TurnContext?.CanFreelyExplore == true || HasExploreStaged
            );
        }

        public DicePool GetStagedDicePool() => _contexts.CheckContext?.DicePool(
            _pcsStagedActions.Values.SelectMany(list => list).ToList()
        ) ?? new DicePool();

        public void Commit()
        {
            // Clear status text first.
            GameEvents.SetStatusText("");

            if (_contexts.CheckContext != null)
                _contexts.CheckContext.CommittedActions = StagedActions.ToList();

            foreach (var action in StagedActions)
            {
                action.Commit();
            }

            HasExploreStaged = false;
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

            UpdateActionButtons();

            // We're done committing actions. Tell the GameFlowManager to continue.
            _gameFlow.Process();
        }

        public void Skip()
        {
            _contexts.CurrentResolvable?.OnSkip();
            Commit();
        }
    }
}
