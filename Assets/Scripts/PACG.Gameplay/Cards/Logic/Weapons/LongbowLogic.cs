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

        public override CheckModifier GetCheckModifier(IStagedAction action)
        {
            // Both powers give +1d8 to combat checks.
            var modifier = new CheckModifier(action.Card)
            {
                RestrictedCategory = CheckCategory.Combat,
                ProhibitedTraits = new[] { "Offhand" }.ToHashSet(),
                AddedDice = new[] { 8 }.ToList()
            };

            if (action.ActionType == ActionType.Discard)
                return modifier;

            modifier.AddedValidSkills = new[] { Skill.Dexterity, Skill.Ranged }.ToList();
            modifier.RestrictedSkills = new[] { Skill.Dexterity, Skill.Ranged }.ToList();
            modifier.AddedTraits = action.Card.Traits;

            return modifier;
        }

        public override void OnCommit(IStagedAction action)
        {
            _contexts.EncounterContext?.AddProhibitedTraits(action.Card.Owner, "Offhand");
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (CanReveal(card))
                actions.Add(new PlayCardAction(card, ActionType.Reveal, ("IsCombat", true)));

            if (CanDiscard(card))
            {
                actions.Add(new PlayCardAction(
                    card,
                    ActionType.Discard,
                    ("IsCombat", true), ("IsFreely", true))
                );
            }

            return actions;
        }

        private bool CanReveal(CardInstance card) =>
            // Reveal power can be used by the current owner while playing cards for a Dexterity or Ranged combat check.
            Check is { IsCombatValid: true }
            && Check.Character == card.Owner
            && !_contexts.CurrentResolvable.IsCardTypeStaged(card.CardType)
            && Check.CanUseSkill(Skill.Dexterity, Skill.Ranged);

        private bool CanDiscard(CardInstance card) => (
            // Discard power can be freely used on another character's combat check while playing cards if the owner is proficient.
            Check is { IsCombatValid: true }
            && Check.Character != card.Owner
            && card.Owner.IsProficient(card.Data)
        );
    }
}
