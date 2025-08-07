
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

        private readonly Dictionary<PlayerCharacter, List<IStagedAction>> pcsStagedActions = new();

        public ActionStagingManager(GameFlowManager gameFlowManager, ContextManager contextManager, CardManager cardManager)
        {
            _gameFlowManager = gameFlowManager;
            _contexts = contextManager;
            _cards = cardManager;
        }

        public void StageAction(IStagedAction action)
        {
            var pcActions = pcsStagedActions.GetValueOrDefault(action.Card.Owner, new());

            if (pcActions.Contains(action))
            {
                Debug.LogWarning($"{action.Card.Data.cardName}.{action} staged multiple times!");
                return;
            }

            // Update game state
            action.Card.Owner.MoveCard(action.Card, action.ActionType); // We need to handle this here so that damage resolvables behave with hand size.
            action.OnStage();

            pcActions.Add(action);
            pcsStagedActions[action.Card.Owner] = pcActions;

            UpdateActionButtonState();

            // Fire event to trigger card display updates.
            GameEvents.RaiseActionStaged(action);
        }

        public void Cancel()
        {
            // TODO: Get currently displayed PC. Use current turn PC for now.
            PlayerCharacter pc = _contexts.TurnContext.CurrentPC;
            foreach (var action in pcsStagedActions[pc])
            {
                action.OnUndo();
                GameEvents.RaiseActionUnstaged(action);
            }

            _cards.RestoreStagedCards();
            pcsStagedActions[pc].Clear();

            UpdateActionButtonState();
        }

        public void UpdateActionButtonState()
        {
            // TODO: Update this for the displayed PC. Use Turn PC until then.
            var pc = _contexts.TurnContext.CurrentPC;
            var stagedActions = pcsStagedActions.GetValueOrDefault(pc) ?? (pcsStagedActions[pc] = new());

            bool canCommit = stagedActions.Count > 0 && (_contexts.ResolutionContext?.IsResolved(stagedActions) ?? true); // We have actions but no resolvable? We can commit!
            bool canSkip = stagedActions.Count == 0 && (_contexts.ResolutionContext?.IsResolved(new()) ?? false); // We don't have any actions and no resolvable to skip, so false!

            StagedActionsState state = new(
                canCancel: stagedActions.Count > 0,
                canCommit: canCommit,
                canSkip: canSkip);
            GameEvents.RaiseStagedActionsStateChanged(state);
        }

        public void Commit()
        {
            foreach (var action in pcsStagedActions.Values.SelectMany(list => list))
            {
                action.Commit();
            }
            pcsStagedActions.Clear();
            _cards.CommitStagedMoves();

            // Decide what to do next based on the context.
            if (_contexts.ResolutionContext != null)
            {
                var currentResolvable = _contexts.ResolutionContext.CurrentResolvable;
                _contexts.EndResolution();
            }

            UpdateActionButtonState();
        }
    }
}
