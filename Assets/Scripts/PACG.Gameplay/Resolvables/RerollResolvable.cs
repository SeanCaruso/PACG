using System.Collections.Generic;

namespace PACG.Gameplay
{
    public class RerollResolvable : IResolvable, ICheckResolvable
    {
        public LogicRegistry LogicRegistry { get; }
        public PlayerCharacter Character { get; }
        public int Difficulty => 0;

        public RerollResolvable(LogicRegistry logicRegistry, PlayerCharacter pc, CheckContext checkContext)
        {
            LogicRegistry = logicRegistry;
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
            var cardLogic = LogicRegistry.GetPlayableLogic(card);
            List<IStagedAction> actions = cardLogic?.GetAvailableActions() ?? new();

            return actions;
        }

        public bool IsResolved(List<IStagedAction> actions) => true; // We can always resolve by skipping.
    }
}