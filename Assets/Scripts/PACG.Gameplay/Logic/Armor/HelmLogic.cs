using System.Collections.Generic;
using System.Linq;
using PACG.Core;

namespace PACG.Gameplay
{
    public class HelmLogic : CardLogicBase
    {
        private readonly ContextManager _contexts;

        public HelmLogic(GameServices gameServices) : base(gameServices) 
        {
            _contexts = gameServices.Contexts;
        }

        public override CheckModifier GetCheckModifier(IStagedAction action)
        {
            return new CheckModifier(action.Card)
            {
                ProhibitedTraits = new[] { "Helm" }.ToHashSet()
            };
        }

        public override void OnCommit(IStagedAction action)
        {
            _contexts.EncounterContext?.AddProhibitedTraits(action.Card.Owner, "Helm");
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (CanReveal(card))
            {
                actions.Add(new PlayCardAction(card, ActionType.Reveal, ("IsFreely", true), ("Damage", 1)));
            }
            return actions;
        }

        private bool CanReveal(CardInstance card) => 
            // We can freely reveal for damage if we have a DamageResolvable for the card's owner with Combat damage, or any type of damage if proficient.
            _contexts.CurrentResolvable is DamageResolvable resolvable
            && (resolvable.DamageType == "Combat" || card.Owner.IsProficient(card.Data))
            && resolvable.PlayerCharacter == card.Owner;
    }
}
