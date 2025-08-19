using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public class EnchantWeaponLogic : CardLogicBase
    {
        // Dependency injections
        private readonly ContextManager _contexts;

        public EnchantWeaponLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        public override void Execute(CardInstance card, IStagedAction action)
        {
            // Add 1d4+# and the Magic trait.
            _contexts.CheckContext?.AddToDicePool(1, 4, _contexts.GameContext.AdventureNumber);
            _contexts.CheckContext?.AddTraits("Magic");
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            // Can freely banish if a weapon has been played on a combat check.
            if (_contexts.CurrentResolvable is CombatResolvable &&
                _contexts.CheckContext?.StagedCardTypes.Contains(PF.CardType.Weapon) == true)
            {
                return new List<IStagedAction>(new[]
                {
                    new PlayCardAction(card, PF.ActionType.Banish, ("IsFreely", true))
                });
            }

            return new List<IStagedAction>();
        }

        public override List<IResolvable> GetRecoveryResolvables(CardInstance card)
        {
            if (!card.Owner.IsProficient(card.Data)) return new List<IResolvable>();

            return new List<IResolvable>
            {
                new SkillResolvable(card, card.Owner, 6, PF.Skill.Arcane, PF.Skill.Divine)
            };
        }
    }
}
