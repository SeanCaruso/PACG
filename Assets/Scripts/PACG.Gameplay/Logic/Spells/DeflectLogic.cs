using System.Collections.Generic;
using System.Linq;
using PACG.Core;

namespace PACG.Gameplay
{
    public class DeflectLogic : CardLogicBase
    {
        // Dependency injection
        private readonly ContextManager _contexts;
        private readonly GameServices _gameServices;
        
        public DeflectLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameServices = gameServices;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();
            
            // Freely banish to reduce a local character's Combat damage by 4.
            if (_contexts.CurrentResolvable is DamageResolvable{ DamageType: "Combat" } resolvable
                && resolvable.PlayerCharacter.LocalCharacters.Contains(card.Owner))
                actions.Add(new PlayCardAction(card, ActionType.Banish, null, ("Damage", 4)));
            
            return actions;
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
