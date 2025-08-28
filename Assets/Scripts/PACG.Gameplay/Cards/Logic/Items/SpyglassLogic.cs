using System.Collections.Generic;
using System.Linq;
using PACG.Core;
using PACG.Data;

namespace PACG.Gameplay
{
    public class SpyglassLogic : CardLogicBase
    {
        private readonly ActionStagingManager _asm;
        private readonly ContextManager _contexts;

        private CheckContext Check => _contexts.CheckContext;

        public SpyglassLogic(GameServices gameServices) : base(gameServices) 
        {
            _asm = gameServices.ASM;
            _contexts = gameServices.Contexts;
        }

        public override CheckModifier GetCheckModifier(IStagedAction action)
        {
            if (action.ActionType != ActionType.Reveal) return null;

            return new CheckModifier(action.Card)
            {
                RestrictedSkills = new[] { Skill.Perception }.ToList(),
                AddedDice = new[] { 6 }.ToList()
            };
        }

        public override void OnCommit(IStagedAction action)
        {
            if (action.ActionType != ActionType.Discard) return;
            _contexts.NewResolvable(new ExamineResolvable(action.Card.Owner.Location, 2, true));
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (CanReveal(card))
                actions.Add(new PlayCardAction(card, ActionType.Reveal));

            // Can discard to examine any time outside resolvables or encounters.
            if (_contexts.CurrentResolvable == null
                && _contexts.EncounterContext == null
                && card.Owner.Location.Count > 0
                && _asm.StagedCards.Count == 0)
            {
                actions.Add(new PlayCardAction(card, ActionType.Discard));
            }

            return actions;
        }

        // Can reveal on your Perception check.
        private bool CanReveal(CardInstance card) =>
            Check != null
            && _contexts.CurrentResolvable is CheckResolvable
            && Check.Character == card.Owner
            && Check.CanUseSkill(Skill.Perception)
            && !Check.StagedCardTypes.Contains(CardType.Item);
    }
}
