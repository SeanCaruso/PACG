using System.Collections.Generic;
using PACG.Core;

namespace PACG.Gameplay
{
    public class ExploreAction : IStagedAction
    {
        public CardInstance Card { get; }
        public ActionType ActionType { get; }

        public bool IsFreely => false; // Doesn't apply to explore actions.
        public Dictionary<string, object> ActionData { get; } = new();

        /// <summary>
        /// Explore actions triggered by something other than playing a card.
        /// </summary>
        public ExploreAction()
        {
        }

        /// <summary>
        /// Explore actions triggered by playing a card.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="actionType"></param>
        public ExploreAction(CardInstance card, ActionType actionType)
        {
            Card = card;
            ActionType = actionType;
        }

        public void Commit() => Card.Logic?.OnCommit(this);
    }
}
