

using System.Collections.Generic;

namespace PACG.Gameplay
{
    public class GiveCardResolvable : IResolvable
    {
        public PlayerCharacter Actor { get; }
        public PlayerCharacter TargetPC { get; }

        public GiveCardResolvable(PlayerCharacter actor, PlayerCharacter targetPc)
        {
            Actor = actor;
            TargetPC = targetPc;
        }

        public List<IStagedAction> GetValidActions()
        {
            List<IStagedAction> actions = new();

            foreach (var card in Actor.Hand)
            {
                actions.AddRange(GetValidActionsForCard(card));
            }

            return actions;
        }

        public List<IStagedAction> GetValidActionsForCard(CardInstance card)
        {
            return new List<IStagedAction> { new GiveCardAction(card, TargetPC) };
        }

        public bool IsResolved(List<IStagedAction> actions)
        {
            // We can always resolve.
            return true;
        }

        public IProcessor CreateProcessor(GameServices gameServices)
        {
            // TODO: Return a processor for GiveCardResolvable
            return null;
        }
    }
}
