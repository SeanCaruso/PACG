using System;
using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public class ThrowingAxeLogic : CardLogicBase
    {
        private CheckContext Check => GameServices.Contexts.CheckContext;

        private PlayCardAction GetRevealAction(CardInstance card) => new(this, card, PF.ActionType.Reveal, ("IsCombat", true));
        private PlayCardAction GetDiscardAction(CardInstance card) => new(this, card, PF.ActionType.Discard, ("IsCombat", true), ("IsFreely", true));

        public ThrowingAxeLogic(GameServices gameServices) : base(gameServices) { }

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
            && Check.Resolvable is CombatResolvable resolvable
            && resolvable.Character == card.Owner
            && !Check.StagedCardTypes.Contains(card.Data.cardType)
            && Check.CheckPhase == CheckPhase.PlayCards
            && Check.CanPlayCardWithSkills(validSkills));

        bool CanDiscard(CardInstance card) => (
            // Discard power can be freely used on a local combat check while playing cards if the owner is proficient.
            Check != null
            && card.Owner.IsProficient(card.Data.cardType)
            && Check.Resolvable is CombatResolvable
            && Check.CheckPhase == CheckPhase.PlayCards
            && true); // TODO: Handle checking for local vs. distant.

        public override void OnStage(CardInstance card, IStagedAction action)
        {
            Check.RestrictValidSkills(card, validSkills);
        }

        public override void OnUndo(CardInstance card, IStagedAction action)
        {
            Check.UndoSkillModification(card);
        }

        public override void Execute(CardInstance card, IStagedAction action)
        {
            var revealAction = GetRevealAction(card);
            var discardAction = GetDiscardAction(card);
            
            if (action.ActionType == revealAction.ActionType && action.Card == card)
            {
                // Reveal to use Strength, Dexterity, Melee, or Ranged + 1d8.
                var (skill, die, bonus) = GameServices.Contexts.TurnContext.CurrentPC.GetBestSkill(PF.Skill.Strength, PF.Skill.Dexterity, PF.Skill.Melee, PF.Skill.Ranged);
                Check.UsedSkill = skill;
                Check.DicePool.AddDice(1, die, bonus);
                Check.DicePool.AddDice(1, 8);
            }

            // Discard to add 1d6.
            if (action.ActionType == discardAction.ActionType && action.Card == card)
            {
                Check.DicePool.AddDice(1, 6);
            }
        }
    }
}
