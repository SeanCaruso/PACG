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

        public override void Execute(CardInstance card, IStagedAction action, DicePool dicePool)
        {
            if (dicePool == null) return;

            // Add 1d4+# and the Magic trait.
            dicePool.AddDice(1, 4, _contexts.GameContext.AdventureNumber);
            _contexts.CheckContext?.AddTraits("Magic");
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            // Can freely banish if a weapon has been played on a combat check.
            if (_contexts.CurrentResolvable is CheckResolvable { HasCombat: true } &&
                _contexts.CheckContext?.StagedCardTypes.Contains(PF.CardType.Weapon) == true)
            {
                return new List<IStagedAction>(new[]
                {
                    new PlayCardAction(card, PF.ActionType.Banish, ("IsFreely", true))
                });
            }

            return new List<IStagedAction>();
        }

        public override IResolvable GetRecoveryResolvable(CardInstance card)
        {
            if (!card.Owner.IsProficient(card.Data)) return null;

            return new CheckResolvable(
                card,
                card.Owner,
                CardUtils.SkillCheck(6, PF.Skill.Arcane, PF.Skill.Divine)
            );
        }
    }
}
