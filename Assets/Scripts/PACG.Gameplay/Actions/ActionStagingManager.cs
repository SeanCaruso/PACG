
using PACG.SharedAPI;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PACG.Gameplay
{
    public class ActionStagingManager
    {
        private CardManager _cards;
        private ContextManager _contexts;
        private GameFlowManager _gameFlow;     // Commit -> Continue processing
        private GameServices _gameServices;    // Passed in to IResolvable.CreateProcessor

        private Dictionary<PlayerCharacter, List<IStagedAction>> PcsStagedActions { get; } = new();
        private Dictionary<CardInstance, CardLocation> OriginalCardLocs { get; } = new();

        public bool CardStaged(CardInstance card) => OriginalCardLocs.Keys.Contains(card);

        public void Iniitalize(GameServices gameServices)
        {
            _cards = gameServices.Cards;
            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
            _gameServices = gameServices;
        }

        public void StageAction(IStagedAction action)
        {
            var pcActions = PcsStagedActions.GetValueOrDefault(action.Card.Owner, new());

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

            // If this is the first staged action for this card, store where it originally came from.
            OriginalCardLocs.TryAdd(action.Card, action.Card.CurrentLocation);

            // We need to handle this here so that damage resolvables behave with hand size.
            _cards.MoveCard(action.Card, action.ActionType);
            
            // Perform all required staging logic.
            action.OnStage();
            pcActions.Add(action);
            PcsStagedActions[action.Card.Owner] = pcActions;

            // If we're attempting a check, add the action's card type if needed.
            _contexts.CheckContext?.StageCardTypeIfNeeded(action);

            // Send the UI event to update Cancel/Commit buttons.
            UpdateActionButtonState();
        }

        public void Cancel()
        {
            // TODO: Get currently displayed PC. Use current turn PC for now.
            PlayerCharacter pc = _contexts.TurnContext.CurrentPC;
            foreach (var action in PcsStagedActions[pc])
            {
                action.OnUndo();
            }

            foreach ((var card, var location) in OriginalCardLocs)
            {
                _cards.MoveCard(card, location);
            }
            GameEvents.RaiseCardLocationsChanged(OriginalCardLocs.Keys.ToList());

            PcsStagedActions[pc].Clear();
            _contexts.CheckContext?.ClearStagedTypes();

            UpdateActionButtonState();
        }

        public void UpdateActionButtonState()
        {
            // TODO: Update this for the displayed PC. Use Turn PC until then.
            var pc = _contexts.TurnContext.CurrentPC;
            var stagedActions = PcsStagedActions.GetValueOrDefault(pc) ?? (PcsStagedActions[pc] = new());

            bool canCommit = stagedActions.Count > 0 && (_contexts.CurrentResolvable?.IsResolved(stagedActions) ?? true); // We have actions but no resolvable? We can commit!
            bool canSkip = stagedActions.Count == 0 && (_contexts.CurrentResolvable?.IsResolved(stagedActions) ?? false); // We don't have any actions and no resolvable to skip, so false!

            StagedActionsState state = new(
                canCancel: stagedActions.Count > 0,
                canCommit: canCommit,
                canSkip: canSkip);
            GameEvents.RaiseStagedActionsStateChanged(state);
        }

        public void Commit()
        {
            foreach (var action in PcsStagedActions.Values.SelectMany(list => list))
            {
                action.Commit(_contexts.CheckContext);
            }
            PcsStagedActions.Clear();

            UpdateActionButtonState();

            // If we have a resolvable, the fact that we committed means that it's been resolved.
            if (_contexts.CurrentResolvable != null)
            {
                var processor = _contexts.CurrentResolvable.CreateProcessor(_gameServices);
                _gameFlow.QueueNextPhase(processor);
                _contexts.EndResolution();
            }

            // We're done committing actions. Tell the GameFlowManager to continue.
            _gameFlow.Process();
        }
    }
}
