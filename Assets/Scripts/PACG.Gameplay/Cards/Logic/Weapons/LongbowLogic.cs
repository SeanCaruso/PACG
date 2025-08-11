using System;
using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public class LongbowLogic : CardLogicBase
    {
        private CheckContext Check => GameServices.Contexts.CheckContext;

        private PlayCardAction GetRevealAction(CardInstance card) => new(this, card, PF.ActionType.Reveal, ("IsCombat", true));
        private PlayCardAction GetDiscardAction(CardInstance card) => new(this, card, PF.ActionType.Discard, ("IsCombat", true), ("IsFreely", true));

        public LongbowLogic(GameServices gameServices) : base(gameServices) { }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (CanReveal(card)) actions.Add(GetRevealAction(card));
            if (CanDiscard(card)) actions.Add(GetDiscardAction(card));
            return actions;
        }

        bool CanReveal(CardInstance card) => (
            // Reveal power can be used by the current owner while playing cards for a Dexterity or Ranged combat check.
            Check != null
            && Check.Resolvable is CombatResolvable resolvable
            && resolvable.Character == card.Owner
            && !Check.StagedCardTypes.Contains(card.Data.cardType)
            && Check.CheckPhase == CheckPhase.PlayCards
            && Check.CanPlayCardWithSkills(PF.Skill.Dexterity, PF.Skill.Ranged)
            );

        bool CanDiscard(CardInstance card) => (
            // Discard power can be freely used on an another character's combat check while playing cards if the owner is proficient.
            Check != null
            && Check.Resolvable is CombatResolvable resolvable
            && resolvable.Character != card.Owner
            && card.Owner.IsProficient(card.Data.cardType)
            && Check.CheckPhase == CheckPhase.PlayCards
            );

        public override void OnStage(CardInstance card, IStagedAction action)
        {
            var revealAction = GetRevealAction(card);
            if (action.ActionType == revealAction.ActionType && action.Card == card)
                Check.RestrictValidSkills(card, PF.Skill.Dexterity, PF.Skill.Ranged);

            GameServices.Contexts.EncounterContext.AddProhibitedTraits(card.Owner, card, "Offhand");
        }

        public override void OnUndo(CardInstance card, IStagedAction action)
        {
            Check.UndoSkillModification(card);
            GameServices.Contexts.EncounterContext.ProhibitedTraits.Remove((card.Owner, card));
        }

        public override void Execute(CardInstance card, IStagedAction action)
        {
            var revealAction = GetRevealAction(card);
            var discardAction = GetDiscardAction(card);
            
            if (action.ActionType == revealAction.ActionType && action.Card == card)
            {
                // Reveal to use Dexterity or Ranged + 1d8.
                var (skill, die, bonus) = GameServices.Contexts.TurnContext.CurrentPC.GetBestSkill(PF.Skill.Dexterity, PF.Skill.Ranged);
                Check.UsedSkill = skill;
                Check.DicePool.AddDice(1, die, bonus);
                Check.DicePool.AddDice(1, 8);
            }

            // Discard to add 1d6.
            if (action.ActionType == discardAction.ActionType && action.Card == card)
            {
                Check.DicePool.AddDice(1, 8);
            }
        }
    }
}
