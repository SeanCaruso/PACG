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

        public override void OnStage(CardInstance card, IStagedAction action)
        {
            if (_contexts.CheckContext == null ||
                _contexts.CurrentResolvable is not ICheckResolvable resolvable)
            {
                return;
            }

            _contexts.CheckContext.SetDieOverride(
                card,
                resolvable.Character.GetBestSkill(PF.Skill.Intelligence, PF.Skill.Wisdom, PF.Skill.Charisma).die
            );
        }

        public override void OnUndo(CardInstance card, IStagedAction action)
        {
            _contexts.CheckContext?.UndoDieOverride(card);
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            // Usable on any non-combat check by the owner.
            if (_contexts.CheckContext is { Resolvable: not CombatResolvable } &&
                !_contexts.CheckContext.StagedCardTypes.Contains(PF.CardType.Item) &&
                _contexts.CheckContext.Character == card.Owner)
            {
                return new List<IStagedAction> { new PlayCardAction(card, PF.ActionType.Recharge) };
            }

            return new List<IStagedAction>();
        }
    }
}
