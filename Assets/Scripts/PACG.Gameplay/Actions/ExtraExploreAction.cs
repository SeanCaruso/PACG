using UnityEngine;

namespace PACG.Gameplay
{
    public class ExtraExploreAction : IStagedAction
    {
        public CardInstance Card { get; } = null;
        public PF.ActionType ActionType { get; }

        public bool IsFreely => throw new System.NotImplementedException();

        /// <summary>
        /// Extra explore actions triggered by something other than playing a card.
        /// </summary>
        public ExtraExploreAction()
        {
        }

        /// <summary>
        /// Extra explore actions triggerd by playing a card.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="actionType"></param>
        public ExtraExploreAction(CardInstance card, PF.ActionType actionType)
        {
            Card = card;
            ActionType = actionType;
        }

        public void Commit(CheckContext checkContext = null) { }

        public void OnStage() { }

        public void OnUndo() { }
    }
}
