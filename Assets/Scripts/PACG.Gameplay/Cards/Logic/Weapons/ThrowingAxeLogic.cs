using System;
using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public class ThrowingAxeLogic : CardLogicBase
    {
        private readonly ContextManager _contexts;

        private CheckContext Check => _contexts.CheckContext;

        private PlayCardAction GetRevealAction(CardInstance card) => new(card, PF.ActionType.Reveal, ("IsCombat", true));
        private PlayCardAction GetDiscardAction(CardInstance card) => new(card, PF.ActionType.Discard, ("IsCombat", true), ("IsFreely", true));

        public ThrowingAxeLogic(GameServices gameServices) : base(gameServices) 
        {
            _contexts = gameServices.Contexts;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (CanReveal(card)) actions.Add(GetRevealAction(card));
            if (CanDiscard(card)) actions.Add(GetDiscardAction(card));
            return actions;
        }

        readonly PF.Skill[] validSkills = { PF.Skill.Strength, PF.Skill.Dexterity, PF.Skill.Melee, PF.Skill.Ranged };
        bool CanReveal(CardInstance card) => (
            // Reveal power can be used by the current owner while playing cards for a Strength, Dexterity, Melee, or Ranged combat check.
            Check != null
            && _contexts.CurrentResolvable is CombatResolvable resolvable
            && resolvable.Character == card.Owner
            && !Check.StagedCardTypes.Contains(card.Data.cardType)
            && Check.CanPlayCardWithSkills(validSkills));

        bool CanDiscard(CardInstance card) => (
            // Discard power can be freely used on a local combat check while playing cards if the owner is proficient.
            Check != null
            && card.Owner.IsProficient(card.Data.cardType)
            && _contexts.CurrentResolvable is CombatResolvable
            && true); // TODO: Handle checking for local vs. distant.

        public override void OnStage(CardInstance card, IStagedAction action)
        {
            Check.RestrictValidSkills(card, validSkills);
        }

        public override void OnUndo(CardInstance card, IStagedAction action)
        {
            Check.UndoSkillModification(card);
        }

        public override void Execute(/*CardInstance card, */IStagedAction action)
        {
            // Reveal to use Strength, Dexterity, Melee, or Ranged + 1d8.       
            if (action.ActionType == PF.ActionType.Reveal)
            {
                var resolvable = _contexts.CurrentResolvable as CombatResolvable;
                var (skill, die, bonus) = resolvable.Character.GetBestSkill(PF.Skill.Strength, PF.Skill.Dexterity, PF.Skill.Melee, PF.Skill.Ranged);
                Check.UsedSkill = skill;
                Check.DicePool.AddDice(1, die, bonus);
                Check.DicePool.AddDice(1, 8);
            }

            // Discard to add 1d6.
            if (action.ActionType == PF.ActionType.Discard)
            {
                Check.DicePool.AddDice(1, 6);
            }
        }
    }
}
