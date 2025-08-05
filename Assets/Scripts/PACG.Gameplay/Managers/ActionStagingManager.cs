
using PACG.SharedAPI;
using PACG.SharedAPI.States;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PACG.Gameplay
{
    public class ActionStagingManager : GameBehaviour
    {
        private readonly Dictionary<PlayerCharacter, List<IStagedAction>> pcsStagedActions = new();

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

            // Fire event
            GameEvents.RaiseActionStaged(action);
        }

        public void Cancel()
        {
            // TODO: Get currently displayed PC. Use current turn PC for now.
            PlayerCharacter pc = Contexts.TurnContext.CurrentPC;
            foreach (var action in pcsStagedActions[pc])
            {
                action.OnUndo();
                GameEvents.RaiseActionUnstaged(action);
            }

            Cards.RestoreStagedCards();
            pcsStagedActions[pc].Clear();

            UpdateActionButtonState();
        }

        public void UpdateActionButtonState()
        {
            // TODO: Update this for the displayed PC. Use Turn PC until then.
            var pc = Contexts.TurnContext.CurrentPC;
            var stagedActions = pcsStagedActions.GetValueOrDefault(pc) ?? (pcsStagedActions[pc] = new());

            bool canCommit = stagedActions.Count > 0 && (Contexts.ResolutionContext?.IsResolved(stagedActions) ?? true); // We have actions but no resolvable? We can commit!
            bool canSkip = stagedActions.Count == 0 && (Contexts.ResolutionContext?.IsResolved(new()) ?? false); // We don't have any actions and no resolvable to skip, so false!

            StagedActionsState state = new(
                isCancelButtonVisible: stagedActions.Count > 0,
                isCommitButtonVisible: canCommit || canSkip,
                useSkipSprite: canSkip);
            GameEvents.RaiseStagedActionsStateChanged(state);
        }

        public void Commit()
        {
            foreach (var action in pcsStagedActions.Values.SelectMany(list => list))
            {
                action.Commit();
            }
            pcsStagedActions.Clear();
            Cards.CommitStagedMoves();

            UpdateActionButtonState();
        }
    }
}
