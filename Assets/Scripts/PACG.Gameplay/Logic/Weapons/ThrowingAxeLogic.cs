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
            // Reveal for Strength/Melee/Dexterity/Ranged +1d6 on a combat check.
            if (CanReveal(card))
            {
                var modifier = new CheckModifier(card)
                {
                    RestrictedCategory = CheckCategory.Combat,
                    AddedTraits = card.Traits,
                    RestrictedSkills = _validSkills.ToList(),
                    AddedDice = new List<int> { 6 }
                };
                actions.Add(new PlayCardAction(card, ActionType.Reveal, modifier, ("IsCombat", true)));
            }

            if (!CanDiscard(card)) return actions;

            // Discard for +1d6 on a local combat check.
            var discardModifier = new CheckModifier(card)
            {
                RestrictedCategory = CheckCategory.Combat,
                AddedDice = new List<int> { 6 }
            };
            actions.Add(new PlayCardAction(card, ActionType.Discard, discardModifier,
                ("IsCombat", true), ("IsFreely", true)));

            return actions;
        }

        private readonly Skill[] _validSkills =
            { Skill.Strength, Skill.Dexterity, Skill.Melee, Skill.Ranged };

        private bool CanReveal(CardInstance card) =>
            // Reveal power can be used by the current owner while playing cards for a Strength, Dexterity, Melee, or Ranged combat check.
            Check is { IsCombatValid: true }
            && _contexts.CurrentResolvable is CheckResolvable { HasCombat: true }
            && Check.Character == card.Owner
            && !_contexts.CurrentResolvable.IsCardTypeStaged(card.CardType)
            && Check.CanUseSkill(_validSkills);

        private bool CanDiscard(CardInstance card) =>
            // Discard power can be freely used on a local combat check while playing cards if the owner is proficient.
            Check is { IsCombatValid: true }
            && _contexts.CurrentResolvable is CheckResolvable { HasCombat: true }
            && card.Owner.IsProficient(card.Data)
            && Check.IsLocal(card.Owner);
    }
}
