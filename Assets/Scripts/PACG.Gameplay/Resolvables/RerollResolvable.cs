using System.Collections.Generic;

namespace PACG.Gameplay
{
    public class RerollResolvable : IResolvable
    {
        public LogicRegistry LogicRegistry { get; }
        public PlayerCharacter PlayerCharacter { get; }

        public RerollResolvable(LogicRegistry logicRegistry, PlayerCharacter playerCharacter, CheckContext checkContext)
        {
            LogicRegistry = logicRegistry;
            PlayerCharacter = playerCharacter;

            // Default option is to not reroll.
            checkContext.ContextData["doReroll"] = false;
        }

        public List<IStagedAction> GetValidActions()
        {
            List<IStagedAction> actions = new();

            foreach (var card in PlayerCharacter.Hand)
            {
                actions.AddRange(GetValidActionsForCard(card));
        }
            foreach (var cardData in PlayerCharacter.DisplayedCards)
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