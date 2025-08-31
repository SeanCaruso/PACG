using System.Collections.Generic;
using PACG.Core;

namespace PACG.Gameplay
{
    public class BracersOfProtectionLogic : CardLogicBase
    {
        // Dependency injection
        private readonly ActionStagingManager _asm;
        private readonly ContextManager _contexts;

        public BracersOfProtectionLogic(GameServices gameServices) : base(gameServices)
        {
            _asm = gameServices.ASM;
            _contexts = gameServices.Contexts;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();

            // Can freely reveal for Combat damage to the owner if not staged already.
            if (_contexts.CurrentResolvable is DamageResolvable { DamageType: "Combat" } combatDamage
                && combatDamage.PlayerCharacter == card.Owner
                && !_asm.CardStaged(card))
            {
                actions.Add(new PlayCardAction(
                    card,
                    ActionType.Reveal,
                    null,
                    ("Damage", 1), ("IsFreely", true)));
            }
            
            // Can recharge for any damage to the owner.
            if (_contexts.CurrentResolvable is DamageResolvable anyDamage
                && anyDamage.PlayerCharacter == card.Owner
                && anyDamage.CanStageType(card.CardType))
            {
                actions.Add(new PlayCardAction(card, ActionType.Recharge, null, ("Damage", 1)));
            }

            return actions;
        }
    }
}
