using System.Collections.Generic;
using PACG.Core;
using PACG.Data;

namespace PACG.Gameplay
{
    public class LightningTouchLogic : CardLogicBase
    {
        // Dependency injection
        private readonly ContextManager _contexts;
        private readonly GameServices _gameServices;

        public LightningTouchLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        public override void OnCommit(IStagedAction action)
        {
            if (_contexts.EncounterContext?.Card.CardType != CardType.Monster) return;

            _contexts.EncounterContext.IgnoreAfterActingPowers = true;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();

            // Playable for Arcane +2d4 on the owner's combat check.
            if (_contexts.CheckContext is not { IsCombatValid: true }
                || _contexts.CurrentResolvable is not CheckResolvable { HasCombat: true } resolvable
                || resolvable.Character != card.Owner
                || _contexts.CurrentResolvable.IsCardTypeStaged(card.CardType)) return actions;
            
            var modifier = new CheckModifier(card)
            {
                RestrictedCategory = CheckCategory.Combat,
                AddedValidSkills = new List<Skill> { Skill.Arcane },
                RestrictedSkills = new List<Skill> { Skill.Arcane },
                AddedDice = new List<int> { 4, 4 },
                AddedTraits = card.Traits
            };
                
            actions.Add(new PlayCardAction(card, ActionType.Banish, modifier, ("IsCombat", true)));

            return actions;
        }

        public override IResolvable GetRecoveryResolvable(CardInstance card)
        {
            if (!card.Owner.IsProficient(card.Data)) return null;

            var resolvable = new CheckResolvable(
                card,
                card.Owner,
                CardUtils.SkillCheck(6, Skill.Arcane))
            {
                OnSuccess = () => card.Owner.Recharge(card),
                OnFailure = () => card.Owner.Discard(card)
            };

            return CardUtils.CreateDefaultRecoveryResolvable(resolvable, _gameServices);
        }
    }
}
