using System.Collections.Generic;
using PACG.Core;

namespace PACG.Gameplay
{
    public class GemOfMentalAcuityLogic : CardLogicBase
    {
        // Dependency injection
        private readonly ContextManager _contexts;

        public GemOfMentalAcuityLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            // Usable on any non-combat check by the owner.
            if (_contexts.CheckContext is not { IsSkillValid: true }
                || (_contexts.CurrentResolvable as CheckResolvable)?.CanStageType(card.CardType) == false
                || _contexts.CheckContext.Character != card.Owner) return new List<IStagedAction>();
            
            var modifier = new CheckModifier(card)
            {
                RestrictedCategory = CheckCategory.Skill,
                DieOverride = card.Owner.GetBestSkill(
                    Skill.Intelligence, Skill.Wisdom, Skill.Charisma
                ).die
            };
                
            return new List<IStagedAction> { new PlayCardAction(card, ActionType.Recharge, modifier) };

        }
    }
}
