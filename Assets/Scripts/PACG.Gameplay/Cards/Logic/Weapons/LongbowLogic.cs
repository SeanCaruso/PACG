using System;
using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    [PlayableLogicFor("Longbow")]
    public class LongbowLogic : CardLogicBase
    {
        private CheckContext Check => GameServices.Contexts.CheckContext;

        private PlayCardAction _revealAction;
        private PlayCardAction RevealAction => _revealAction ??= new(this, Card, PF.ActionType.Reveal, ("IsCombat", true));

        private PlayCardAction _discardAction;
        private PlayCardAction DiscardAction => _discardAction ??= new(this, Card, PF.ActionType.Discard, ("IsCombat", true), ("IsFreely", true));

        public LongbowLogic(GameServices gameServices) : base(gameServices) { }

        protected override List<IStagedAction> GetAvailableCardActions()
        {
            List<IStagedAction> actions = new();
            if (CanReveal) actions.Add(RevealAction);
            if (CanDiscard) actions.Add(DiscardAction);
            return actions;
        }

        bool CanReveal => (
            // Reveal power can be used by the current owner while playing cards for a Dexterity or Ranged combat check.
            Check != null
            && Check.Resolvable is CombatResolvable resolvable
            && resolvable.Character == Card.Owner
            && !Check.StagedCardTypes.Contains(Card.Data.cardType)
            && Check.CheckPhase == CheckPhase.PlayCards
            && Check.CanPlayCardWithSkills(PF.Skill.Dexterity, PF.Skill.Ranged)
            );

        bool CanDiscard => (
            // Discard power can be freely used on an another character's combat check while playing cards if the owner is proficient.
            Check != null
            && Check.Resolvable is CombatResolvable resolvable
            && resolvable.Character != Card.Owner
            && Card.Owner.IsProficient(Card.Data.cardType)
            && Check.CheckPhase == CheckPhase.PlayCards
            );

        public override void OnStage(IStagedAction action)
        {
            if (action == RevealAction)
                Check.RestrictValidSkills(Card, PF.Skill.Dexterity, PF.Skill.Ranged);

            GameServices.Contexts.EncounterContext.AddProhibitedTraits(Card.Owner, Card, "Offhand");
        }

        public override void OnUndo(IStagedAction action)
        {
            Check.UndoSkillModification(Card);
            GameServices.Contexts.EncounterContext.ProhibitedTraits.Remove((Card.Owner, Card));
        }

        public override void Execute(IStagedAction action)
        {
            if (action == RevealAction)
            {
                // Reveal to use Dexterity or Ranged + 1d8.
                var (skill, die, bonus) = GameServices.Contexts.TurnContext.CurrentPC.GetBestSkill(PF.Skill.Dexterity, PF.Skill.Ranged);
                Check.UsedSkill = skill;
                Check.DicePool.AddDice(1, die, bonus);
                Check.DicePool.AddDice(1, 8);
            }

            // Discard to add 1d6.
            if (action == DiscardAction)
            {
                Check.DicePool.AddDice(1, 8);
            }
        }
    }
}
