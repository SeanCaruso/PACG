using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public class SpyglassLogic : CardLogicBase
    {
        private readonly ContextManager _contexts;

        private CheckContext Check => _contexts.CheckContext;

        public SpyglassLogic(GameServices gameServices) : base(gameServices) 
        {
            _contexts = gameServices.Contexts;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (CanReveal(card))
                actions.Add(new PlayCardAction(card, PF.ActionType.Reveal));

            // Can discard to examine any time outside resolvables or encounters.
            if (_contexts.CurrentResolvable == null &&
                _contexts.EncounterContext == null &&
                card.Owner.Location.Count > 0)
            {
                actions.Add(new PlayCardAction(card, PF.ActionType.Discard));
            }

            return actions;
        }

        // Can reveal on your Perception check.
        private bool CanReveal(CardInstance card) => (
            Check != null &&
            Check.Character == card.Owner &&
            Check.CanUseSkill(PF.Skill.Perception) &&
            !Check.StagedCardTypes.Contains(PF.CardType.Item)
            );

        public override void OnStage(CardInstance card, IStagedAction action)
        {
            if (action.ActionType == PF.ActionType.Reveal)
                Check.RestrictValidSkills(card, PF.Skill.Perception);
        }

        public override void OnUndo(CardInstance card, IStagedAction action)
        {
            if (action.ActionType == PF.ActionType.Reveal)
                Check.UndoSkillModification(card);
        }

        public override void Execute(CardInstance card, IStagedAction action)
        {
            switch (action.ActionType)
            {
                // Reveal to add 1d6 on your Perception check.
                case PF.ActionType.Reveal:
                    Check.DicePool.AddDice(1, 8);
                    break;
                // Discard to examine the top 2 cards of your location and return them in any order.
                case PF.ActionType.Discard:
                    _contexts.NewResolvable(new ExamineResolvable(card.Owner.Location, 2, true));
                    break;
            }
        }
    }
}
