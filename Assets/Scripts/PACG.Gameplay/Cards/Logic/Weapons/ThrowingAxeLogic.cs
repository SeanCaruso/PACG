using System.Collections.Generic;
using System.Linq;

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

        public override CheckModifier GetCheckModifier(IStagedAction action)
        {
            var modifier = new CheckModifier(action.Card)
            {
                RestrictedCategory = CheckCategory.Combat
            };

            switch (action.ActionType)
            {
                // Reveal to use Strength, Dexterity, Melee, or Ranged + 1d6.
                case PF.ActionType.Reveal:
                    modifier.AddedTraits.AddRange(action.Card.Traits);
                    modifier.RestrictedSkills.AddRange(_validSkills);
                    modifier.AddedDice.Add(6);
                    break;
                // Discard to add 1d6.
                case PF.ActionType.Discard:
                    modifier.AddedDice.Add(6);
                    break;
            }
            
            return modifier;
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
            && _contexts.CurrentResolvable is CheckResolvable { HasCombat: true }
            && Check.Character == card.Owner
            && !Check.StagedCardTypes.Contains(card.Data.cardType)
            && Check.CanUseSkill(_validSkills);

        private bool CanDiscard(CardInstance card) =>
            // Discard power can be freely used on a local combat check while playing cards if the owner is proficient.
            Check is { IsCombatValid: true }
            && _contexts.CurrentResolvable is CheckResolvable { HasCombat: true }
            && card.Owner.IsProficient(card.Data)
            && Check.IsLocal(card.Owner);
    }
}
