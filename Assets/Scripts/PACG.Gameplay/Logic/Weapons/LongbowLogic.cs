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

        public override void OnCommit(IStagedAction action)
        {
            _contexts.EncounterContext?.AddProhibitedTraits(action.Card.Owner, "Offhand");
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (CanReveal(card))
            {
                var revealModifier = new CheckModifier(card)
                {
                    RestrictedCategory = CheckCategory.Combat,
                    ProhibitedTraits = new[] { "Offhand" }.ToHashSet(),
                    AddedDice = new List<int> { 8 },
                    AddedValidSkills = new List<Skill> { Skill.Dexterity, Skill.Ranged },
                    RestrictedSkills = new List<Skill> { Skill.Dexterity, Skill.Ranged },
                    AddedTraits = card.Traits
                };
                actions.Add(new PlayCardAction(card, ActionType.Reveal, revealModifier, ("IsCombat", true)));
            }

            if (!CanDiscard(card)) return actions;
            
            var modifier = new CheckModifier(card)
            {
                RestrictedCategory = CheckCategory.Combat,
                ProhibitedTraits = new[] { "Offhand" }.ToHashSet(),
                AddedDice = new[] { 8 }.ToList()
            };
                
            actions.Add(new PlayCardAction(
                card,
                ActionType.Discard,
                modifier,
                ("IsCombat", true), ("IsFreely", true))
            );

            return actions;
        }

        private bool CanReveal(CardInstance card) =>
            // Reveal power can be used by the current owner while playing cards for a Dexterity or Ranged combat check.
            Check is { IsCombatValid: true }
            && Check.Character == card.Owner
            && _contexts.CurrentResolvable.CanStageType(card.CardType)
            && Check.CanUseSkill(Skill.Dexterity, Skill.Ranged);

        private bool CanDiscard(CardInstance card) => (
            // Discard power can be freely used on another character's combat check while playing cards if the owner is proficient.
            Check is { IsCombatValid: true }
            && Check.Character != card.Owner
            && card.Owner.IsProficient(card.Data)
        );
    }
}
