using PACG.SharedAPI;

namespace PACG.Gameplay
{
    public class MoveAction : IStagedAction
    {
        public CardInstance Card { get; }
        public PF.ActionType ActionType { get; }

        public bool IsFreely => false; // Doesn't apply to Move actions.

        /// <summary>
        /// Move actions triggered by something other than playing a card.
        /// </summary>
        public MoveAction()
        {
        }

        /// <summary>
        /// Move actions triggered by playing a card.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="actionType"></param>
        public MoveAction(CardInstance card, PF.ActionType actionType)
        {
            Card = card;
            ActionType = actionType;
        }

        public void Commit(CheckContext checkContext, DicePool dicePool) => Card.Logic?.Execute(Card, this, dicePool);

        public void OnStage() => GameEvents.SetStatusText("Move?");

        public void OnUndo() => GameEvents.SetStatusText("");
    }
}
