using System.Collections.Generic;

namespace PACG.Gameplay
{
    public class RerollResolvable : IResolvable, ICheckResolvable
    {
        public PlayerCharacter Character { get; }
        public int Difficulty => 0;

        public RerollResolvable(PlayerCharacter pc, CheckContext checkContext)
        {
            Character = pc;

            // Default option is to not reroll.
            checkContext.ContextData["doReroll"] = false;
        }

        public List<IStagedAction> GetValidActions()
        {
            List<IStagedAction> actions = new();

            foreach (var card in Character.Hand)
            {
                actions.AddRange(GetValidActionsForCard(card));
        }
            foreach (var cardData in Character.DisplayedCards)
            {
                actions.AddRange(GetValidActionsForCard(cardData));
            }

            return actions;
        }

        public List<IStagedAction> GetValidActionsForCard(CardInstance card)
        {
            List<IStagedAction> actions = card.Logic?.GetAvailableActions(card) ?? new();

            return actions;
        }

        public bool IsResolved(List<IStagedAction> actions) => true; // We can always resolve by skipping.

        public IProcessor CreateProcessor(GameServices gameServices)
        {
            // TODO: Return a processor for RerollResolvable
            return null;
        }
    }
}