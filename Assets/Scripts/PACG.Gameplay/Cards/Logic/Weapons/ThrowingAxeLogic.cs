using System.Collections.Generic;
using System.Linq;
using PACG.Core;

namespace PACG.Gameplay
{
    public class ThrowingAxeLogic : CardLogicBase
    {
        private readonly ContextManager _contexts;

        private CheckContext Check => _contexts.CheckContext;

        public ThrowingAxeLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (CanReveal(card))
                actions.Add(new PlayCardAction(card, PF.ActionType.Reveal, ("IsCombat", true)));
            if (CanDiscard(card))
                actions.Add(new PlayCardAction(card, PF.ActionType.Discard, ("IsCombat", true), ("IsFreely", true)));
            return actions;
        }

        private readonly PF.Skill[] _validSkills =
            { PF.Skill.Strength, PF.Skill.Dexterity, PF.Skill.Melee, PF.Skill.Ranged };

        private bool CanReveal(CardInstance card) =>
            // Reveal power can be used by the current owner while playing cards for a Strength, Dexterity, Melee, or Ranged combat check.
            Check is { IsCombatValid: true }
            && Check.Character == card.Owner
            && !Check.StagedCardTypes.Contains(card.Data.cardType)
            && Check.CanUseSkill(_validSkills);

        private bool CanDiscard(CardInstance card) =>
            // Discard power can be freely used on a local combat check while playing cards if the owner is proficient.
            Check is { IsCombatValid: true }
            && card.Owner.IsProficient(card.Data)
            && Check.IsLocal(card.Owner);

        public override void OnStage(CardInstance card, IStagedAction action)
        {
            Check.RestrictCheckCategory(card, CheckCategory.Combat);
            Check.AddValidSkills(card, _validSkills);
            Check.RestrictValidSkills(card, _validSkills);
            
            // Add traits if revealed to use for the combat check.
            if (action.ActionType == PF.ActionType.Reveal)
                Check.AddTraits(card);
        }

        public override void OnUndo(CardInstance card, IStagedAction action)
        {
            Check.UndoCheckRestriction(card);
            Check.UndoSkillModification(card);
            
            if (action.ActionType == PF.ActionType.Reveal)
                Check.RemoveTraits(card);
        }

        public override void Execute(CardInstance card, IStagedAction action, DicePool dicePool)
        {
            switch (action.ActionType)
            {
                // Reveal to use Strength, Dexterity, Melee, or Ranged + 1d8.       
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
