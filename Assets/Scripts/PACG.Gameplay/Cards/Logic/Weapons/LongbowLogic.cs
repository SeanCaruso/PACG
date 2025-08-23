using System.Collections.Generic;
using System.Linq;
using PACG.Core;

namespace PACG.Gameplay
{
    public class LongbowLogic : CardLogicBase
    {
        private readonly ContextManager _contexts;

        private CheckContext Check => _contexts.CheckContext;

        public LongbowLogic(GameServices gameServices) : base(gameServices) 
        {
            _contexts = gameServices.Contexts;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (CanReveal(card))
                actions.Add(new PlayCardAction(card, PF.ActionType.Reveal, ("IsCombat", true)));
            
            if (CanDiscard(card))
            {
                actions.Add(new PlayCardAction(
                    card,
                    PF.ActionType.Discard,
                    ("IsCombat", true), ("IsFreely", true))
                );
            }

            return actions;
        }

        private bool CanReveal(CardInstance card) => 
            // Reveal power can be used by the current owner while playing cards for a Dexterity or Ranged combat check.
            Check is { IsCombatValid: true }
            && Check.Character == card.Owner
            && !Check.StagedCardTypes.Contains(card.Data.cardType)
            && Check.CanUseSkill(PF.Skill.Dexterity, PF.Skill.Ranged);

        private bool CanDiscard(CardInstance card) => (
            // Discard power can be freely used on another character's combat check while playing cards if the owner is proficient.
            Check is { IsCombatValid: true }
            && Check.Character != card.Owner
            && card.Owner.IsProficient(card.Data)
        );

        public override void OnStage(CardInstance card, IStagedAction action)
        {
            if (action.ActionType == PF.ActionType.Reveal)
            {
                Check.RestrictCheckCategory(card, CheckCategory.Combat);
                Check.AddValidSkills(card, PF.Skill.Dexterity, PF.Skill.Ranged);
                Check.RestrictValidSkills(card, PF.Skill.Dexterity, PF.Skill.Ranged);
                
                Check.AddTraits(card);
            }

            _contexts.EncounterContext.AddProhibitedTraits(card.Owner, card, "Offhand");
        }

        public override void OnUndo(CardInstance card, IStagedAction action)
        {
            if (action.ActionType == PF.ActionType.Reveal)
            {
                Check.UndoCheckRestriction(card);
                Check.UndoSkillModification(card);
                
                Check.RemoveTraits(card);
            }

            _contexts.EncounterContext.UndoProhibitedTraits(card.Owner, card);
        }

        public override void Execute(CardInstance card, IStagedAction action, DicePool dicePool)
        {
            switch (action.ActionType)
            {
                // Reveal to use Dexterity or Ranged + 1d8.        
                case PF.ActionType.Reveal:
                    dicePool.AddDice(1, 8);
                    break;
                // Discard to add 1d6.
                case PF.ActionType.Discard:
                    dicePool.AddDice(1, 6);
                    break;
            }
        }
    }
}
