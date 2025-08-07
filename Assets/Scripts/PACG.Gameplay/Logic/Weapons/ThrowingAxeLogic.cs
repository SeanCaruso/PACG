using System;
using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    [PlayableLogicFor("ThrowingAxe")]
    public class ThrowingAxeLogic : CardLogicBase, IPlayableLogic
    {
        private CheckContext Check => GameServices.Contexts.CheckContext;

        private PlayCardAction _revealAction;
        private PlayCardAction RevealAction => _revealAction ??= new(this, Card, PF.ActionType.Reveal, ("IsCombat", true));

        private PlayCardAction _discardAction;
        private PlayCardAction DiscardAction => _discardAction ??= new(this, Card, PF.ActionType.Discard, ("IsCombat", true), ("IsFreely", true));

        public ThrowingAxeLogic(GameServices gameServices) : base(gameServices) { }

        protected override List<IStagedAction> GetAvailableCardActions()
        {
            List<IStagedAction> actions = new();
            if (CanReveal) actions.Add(RevealAction);
            if (CanDiscard) actions.Add(DiscardAction);
            return actions;
        }

        readonly PF.Skill[] validSkills = { PF.Skill.Strength, PF.Skill.Dexterity, PF.Skill.Melee, PF.Skill.Ranged };
        bool CanReveal => (
            // Reveal power can be used by the current owner while playing cards for a Strength, Dexterity, Melee, or Ranged combat check.
            Check != null
            && Check.Resolvable is CombatResolvable resolvable
            && resolvable.Character == Card.Owner
            && !Check.StagedCardTypes.Contains(Card.Data.cardType)
            && Check.CheckPhase == CheckPhase.PlayCards
            && Check.CanPlayCardWithSkills(validSkills));

        bool CanDiscard => (
            // Discard power can be freely used on a local combat check while playing cards if the owner is proficient.
            Check != null
            && Card.Owner.IsProficient(Card.Data.cardType)
            && Check.Resolvable is CombatResolvable
            && Check.CheckPhase == CheckPhase.PlayCards
            && true); // TODO: Handle checking for local vs. distant.

        void IPlayableLogic.OnStage(IStagedAction action)
        {
            Check.RestrictValidSkills(Card, validSkills);
        }

        void IPlayableLogic.OnUndo(IStagedAction action)
        {
            Check.UndoSkillModification(Card);
        }

        void IPlayableLogic.Execute(IStagedAction action)
        {
            if (action == RevealAction)
            {
                // Reveal to use Strength, Dexterity, Melee, or Ranged + 1d8.
                var (skill, die, bonus) = GameServices.Contexts.TurnContext.CurrentPC.GetBestSkill(PF.Skill.Strength, PF.Skill.Dexterity, PF.Skill.Melee, PF.Skill.Ranged);
                Check.UsedSkill = skill;
                Check.DicePool.AddDice(1, die, bonus);
                Check.DicePool.AddDice(1, 8);
            }

            // Discard to add 1d6.
            if (action == DiscardAction)
            {
                Check.DicePool.AddDice(1, 6);
            }
        }
    }
}
