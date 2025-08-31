using System.Collections.Generic;
using System.Linq;
using PACG.Core;
using PACG.Data;

namespace PACG.Gameplay
{
    public class EnchantWeaponLogic : CardLogicBase
    {
        // Dependency injections
        private readonly ActionStagingManager _asm;
        private readonly ContextManager _contexts;
        private readonly GameServices _gameServices;

        public EnchantWeaponLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameServices = gameServices;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            // Can freely banish for +1d4 if a weapon has been played on a combat check.
            if (_contexts.CurrentResolvable is not CheckResolvable { HasCombat: true }
                || _asm.StagedCards.All(c => c.CardType != CardType.Weapon)) return new List<IStagedAction>();
            
            var modifier = new CheckModifier(card)
            {
                AddedDice = new List<int> { 4 },
                AddedBonus = _contexts.GameContext.AdventureNumber,
                AddedTraits = new List<string> { "Magic" }
            };
            return new List<IStagedAction>(new[]
            {
                new PlayCardAction(card, ActionType.Banish, modifier, ("IsFreely", true))
            });

        }

        public override IResolvable GetRecoveryResolvable(CardInstance card)
        {
            if (!card.Owner.IsProficient(card.Data)) return null;

            var resolvable = new CheckResolvable(
                card,
                card.Owner,
                CardUtils.SkillCheck(6, Skill.Arcane, Skill.Divine),
                _gameServices)
            {
                OnSuccess = () => card.Owner.Recharge(card),
                OnFailure = () => card.Owner.Discard(card)
            };

            return CardUtils.CreateDefaultRecoveryResolvable(resolvable, _gameServices);
        }
    }
}
