using System.Collections.Generic;
using System.Linq;

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

        public List<IStagedAction> GetAdditionalActionsForCard(CardInstance card)
        {
            // Only provide the give card action if the card is in the actor's hand
            if (Actor.Hand.Contains(card))
                return new List<IStagedAction> { new GiveCardAction(card, TargetPC) };
            return new List<IStagedAction>();
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
