using System.Collections.Generic;
using System.Linq;

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

        public override CheckModifier GetCheckModifier(IStagedAction action)
        {
            var modifier = new CheckModifier(action.Card)
            {
                RestrictedCategory = CheckCategory.Skill,
                DieOverride = action.Card.Owner.GetBestSkill(
                    PF.Skill.Intelligence, PF.Skill.Wisdom, PF.Skill.Charisma
                ).die
            };

            return modifier;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            // Usable on any non-combat check by the owner.
            if (_contexts.CheckContext is { IsSkillValid: true }
                && !_contexts.CheckContext.StagedCardTypes.Contains(PF.CardType.Item)
                && _contexts.CheckContext.Character == card.Owner)
            {
                return new List<IStagedAction> { new PlayCardAction(card, PF.ActionType.Recharge) };
            }

            return new List<IStagedAction>();
        }
    }
}
