using System.Collections.Generic;
using System.Linq;
using PACG.Core;
using PACG.Data;

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

        public override CheckModifier GetCheckModifier(IStagedAction action)
        {
            var modifier = new CheckModifier(action.Card);
            modifier.AddedDice.Add(4);
            modifier.AddedBonus += _contexts.GameContext.AdventureNumber;
            modifier.AddedTraits.Add("Magic");
            return modifier;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            // Can freely banish if a weapon has been played on a combat check.
            if (_contexts.CurrentResolvable is CheckResolvable { HasCombat: true } &&
                _contexts.CheckContext?.StagedCardTypes.Contains(CardType.Weapon) == true)
            {
                return new List<IStagedAction>(new[]
                {
                    new PlayCardAction(card, ActionType.Banish, ("IsFreely", true))
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
                CardUtils.SkillCheck(6, Skill.Arcane, Skill.Divine)
            );
        }
    }
}
