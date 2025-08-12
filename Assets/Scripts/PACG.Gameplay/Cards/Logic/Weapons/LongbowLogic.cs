using System;
using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public class LongbowLogic : CardLogicBase
    {
        private readonly ContextManager _contexts;

        private CheckContext Check => _contexts.CheckContext;

        private PlayCardAction GetRevealAction(CardInstance card) => new(card, PF.ActionType.Reveal, ("IsCombat", true));
        private PlayCardAction GetDiscardAction(CardInstance card) => new(card, PF.ActionType.Discard, ("IsCombat", true), ("IsFreely", true));

        public LongbowLogic(GameServices gameServices) : base(gameServices) 
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

        bool CanReveal(CardInstance card) => (
            // Reveal power can be used by the current owner while playing cards for a Dexterity or Ranged combat check.
            Check != null
            && _contexts.CurrentResolvable is CombatResolvable resolvable
            && resolvable.Character == card.Owner
            && !Check.StagedCardTypes.Contains(card.Data.cardType)
            && Check.CanPlayCardWithSkills(PF.Skill.Dexterity, PF.Skill.Ranged)
            );

        bool CanDiscard(CardInstance card) => (
            // Discard power can be freely used on an another character's combat check while playing cards if the owner is proficient.
            Check != null
            && Check.Resolvable is CombatResolvable resolvable
            && resolvable.Character != card.Owner
            && card.Owner.IsProficient(card.Data.cardType)
            );

        public override void OnStage(CardInstance card, IStagedAction action)
        {
            var revealAction = GetRevealAction(card);
            if (action.ActionType == revealAction.ActionType && action.Card == card)
                Check.RestrictValidSkills(card, PF.Skill.Dexterity, PF.Skill.Ranged);

            _contexts.EncounterContext.AddProhibitedTraits(card.Owner, card, "Offhand");
        }

        public override void OnUndo(CardInstance card, IStagedAction action)
        {
            Check.UndoSkillModification(card);
            _contexts.EncounterContext.UndoProhibitedTraits(card.Owner, card);
        }

        public override void Execute(IStagedAction action)
        {
            // Reveal to use Dexterity or Ranged + 1d8.        
            if (action.ActionType == PF.ActionType.Reveal)
            {
                var (skill, die, bonus) = _contexts.TurnContext.CurrentPC.GetBestSkill(PF.Skill.Dexterity, PF.Skill.Ranged);
                Check.UsedSkill = skill;
                Check.DicePool.AddDice(1, die, bonus);
                Check.DicePool.AddDice(1, 8);
            }

            // Discard to add 1d6.
            if (action.ActionType == PF.ActionType.Discard)
            {
                Check.DicePool.AddDice(1, 8);
            }
        }
    }
}
