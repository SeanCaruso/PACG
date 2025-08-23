using PACG.Core;
using PACG.SharedAPI;

namespace PACG.Gameplay
{
    public class ExploreAction : IStagedAction
    {
        public CardInstance Card { get; }
        public PF.ActionType ActionType { get; }

        public bool IsFreely => false; // Doesn't apply to explore actions.

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
        public ExploreAction(CardInstance card, PF.ActionType actionType)
        {
            Card = card;
            ActionType = actionType;
        }

        public void Commit(CheckContext checkContext, DicePool dicePool) => Card.Logic?.Execute(Card, this, dicePool);

        public void OnStage() => GameEvents.SetStatusText("Explore?");

        public void OnUndo() => GameEvents.SetStatusText("");
    }
}
