using System.Collections.Generic;
using System.Linq;
using PACG.Core;
using PACG.Data;

namespace PACG.Gameplay
{
    public class LightningTouchLogic : CardLogicBase
    {
        // Dependency injection
        private readonly ContextManager _contexts;

        public LightningTouchLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        public override CheckModifier GetCheckModifier(IStagedAction action)
        {
            if (_contexts.CurrentResolvable is not CheckResolvable) return null;

            return new CheckModifier(action.Card)
            {
                RestrictedCategory = CheckCategory.Combat,
                AddedValidSkills = new List<Skill> { Skill.Arcane },
                RestrictedSkills = new List<Skill> { Skill.Arcane },
                AddedDice = new List<int> { 4, 4 },
                AddedTraits = action.Card.Traits
            };
        }

        public override void OnCommit(IStagedAction action)
        {
            if (_contexts.EncounterContext?.Card.CardType != CardType.Monster) return;

            _contexts.EncounterContext.IgnoreAfterActingPowers = true;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();

            // Playable on the owner's combat check.
            if (_contexts.CheckContext is { IsCombatValid: true }
                && _contexts.CurrentResolvable is CheckResolvable { HasCombat: true } resolvable
                && resolvable.Character == card.Owner
                && _contexts.CheckContext?.StagedCardTypes.Count(t => t == card.Data.cardType) == 0)
            {
                actions.Add(new PlayCardAction(card, ActionType.Banish));
            }

            return actions;
        }

        public override IResolvable GetRecoveryResolvable(CardInstance card)
        {
            if (!card.Owner.IsProficient(card.Data)) return null;

            return new CheckResolvable(
                card,
                card.Owner,
                CardUtils.SkillCheck(6, Skill.Arcane)
            );
        }
    }
}
