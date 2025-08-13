using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public class GiveCardResolvable : BaseResolvable
    {
        public PlayerCharacter Actor { get; }
        public PlayerCharacter TargetPC { get; }

        public GiveCardResolvable(PlayerCharacter actor, PlayerCharacter targetPc)
        {
            Actor = actor;
            TargetPC = targetPc;
        }

        public override List<IStagedAction> GetAdditionalActionsForCard(CardInstance card)
        {
            // Only provide the give card action if the card is in the actor's hand
            if (Actor.Hand.Contains(card))
                return new List<IStagedAction> { new GiveCardAction(card, TargetPC) };
            return new List<IStagedAction>();
        }
    }
}
