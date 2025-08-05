using System;
using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    [PlayableLogicFor("Longbow")]
    public class LongbowLogic : CardLogicBase, IPlayableLogic
    {

        private PlayCardAction _revealAction;
        private PlayCardAction RevealAction => _revealAction ??= new(this, Card, PF.ActionType.Reveal, ("IsCombat", true));

        private PlayCardAction _discardAction;
        private PlayCardAction DiscardAction => _discardAction ??= new(this, Card, PF.ActionType.Discard, ("IsCombat", true), ("IsFreely", true));

        List<IStagedAction> IPlayableLogic.GetAvailableCardActions()
        {
            List<IStagedAction> actions = new();
            if (CanReveal) actions.Add(RevealAction);
            if (CanDiscard) actions.Add(DiscardAction);
            return actions;
        }

        bool CanReveal => (
            // Reveal power can be used by the current owner while playing cards for a Dexterity or Ranged combat check.
            Contexts.CheckContext != null
            && Contexts.CheckContext.CheckPC == Card.Owner
            && !Contexts.CheckContext.StagedCardTypes.Contains(Card.Data.cardType)
            && Contexts.CheckContext.CheckCategory == CheckCategory.Combat
            && Contexts.CheckContext.CheckPhase == CheckPhase.PlayCards
            && Contexts.CheckContext.CanPlayCardWithSkills(PF.Skill.Dexterity, PF.Skill.Ranged)
            );

        bool CanDiscard => (
            // Discard power can be freely used on an another character's combat check while playing cards if the owner is proficient.
            Contexts.CheckContext != null
            && Card.Owner != Contexts.CheckContext.CheckPC
            && Card.Owner.IsProficient(Card.Data.cardType)
            && Contexts.CheckContext.CheckCategory == CheckCategory.Combat
            && Contexts.CheckContext.CheckPhase == CheckPhase.PlayCards
            );

        void IPlayableLogic.OnStage(IStagedAction action)
        {
            if (action == RevealAction)
                Contexts.CheckContext.RestrictValidSkills(Card, PF.Skill.Dexterity, PF.Skill.Ranged);

            Contexts.EncounterContext.AddProhibitedTraits(Card.Owner, Card, "Offhand");
        }

        void IPlayableLogic.OnUndo(IStagedAction action)
        {
            Contexts.CheckContext.UndoSkillModification(Card);
            Contexts.EncounterContext.ProhibitedTraits.Remove((Card.Owner, Card));
        }

        void IPlayableLogic.Execute(IStagedAction action)
        {
            if (action == RevealAction)
            {
                // Reveal to use Dexterity or Ranged + 1d8.
                var (skill, die, bonus) = Contexts.TurnContext.CurrentPC.GetBestSkill(PF.Skill.Dexterity, PF.Skill.Ranged);
                Contexts.CheckContext.UsedSkill = skill;
                Contexts.CheckContext.DicePool.AddDice(1, die, bonus);
                Contexts.CheckContext.DicePool.AddDice(1, 8);
            }

            // Discard to add 1d6.
            if (action == DiscardAction)
            {
                Contexts.CheckContext.DicePool.AddDice(1, 8);
            }
        }
    }
}
