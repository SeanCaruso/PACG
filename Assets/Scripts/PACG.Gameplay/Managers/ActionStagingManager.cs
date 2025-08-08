
using PACG.SharedAPI;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PACG.Gameplay
{
    public class ActionStagingManager
    {
        private readonly GameFlowManager _gameFlowManager;
        private readonly ContextManager _contexts;
        private readonly CardManager _cards;

        private Dictionary<PlayerCharacter, List<IStagedAction>> PcsStagedActions { get; } = new();
        private Dictionary<CardInstance, CardLocation> OriginalCardLocs { get; } = new();

        public ActionStagingManager(GameFlowManager gameFlowManager, ContextManager contextManager, CardManager cardManager)
        {
            _gameFlowManager = gameFlowManager;
            _contexts = contextManager;
            _cards = cardManager;
        }

        public void StageAction(IStagedAction action)
        {
            var pcActions = PcsStagedActions.GetValueOrDefault(action.Card.Owner, new());

            if (pcActions.Contains(action))
            {
                Debug.LogWarning($"{action.Card.Data.cardName}.{action} staged multiple times!");
                return;
            }

            // If this is the first staged action for this card, store where it originally came from.
            OriginalCardLocs.TryAdd(action.Card, action.Card.CurrentLocation);

            // Update game state
            _cards.MoveCard(action.Card, action.ActionType); // We need to handle this here so that damage resolvables behave with hand size.
            action.OnStage();

            pcActions.Add(action);
            PcsStagedActions[action.Card.Owner] = pcActions;

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
                action.Commit();
            }
            PcsStagedActions.Clear();

            // If we were able to commit with a current resolvable, that resolvable has now been resolved.
            if (_contexts.CurrentResolvable != null)
            {
                _gameFlowManager.QueueProcessorFor(_contexts.CurrentResolvable);
                _contexts.EndResolution();
            }

            UpdateActionButtonState();

            // We're done committing actions. Tell the GameFlowManager to continue.
            _gameFlowManager.Process();
        }
    }
}
