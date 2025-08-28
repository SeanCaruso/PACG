using System.Collections.Generic;
using System.Linq;
using PACG.Core;

namespace PACG.Gameplay
{
    public class ForceMissileLogic : CardLogicBase
    {
        // Dependency injection
        private readonly ContextManager _contexts;
        private readonly GameServices _gameServices;
        
        public ForceMissileLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameServices = gameServices;
        }

        public override CheckModifier GetCheckModifier(IStagedAction action)
        {
            if (_contexts.CurrentResolvable is not CheckResolvable resolvable) return null;
            
            var modifier = new CheckModifier(action.Card);
            modifier.AddedDice.AddRange(new [] { 4, 4 }.ToList());
            modifier.AddedTraits.AddRange(new[] { "Attack", "Force", "Magic" });
            modifier.RestrictedCategory = CheckCategory.Combat;
            
            if (action.Card.Owner != resolvable.Character) return modifier;
            
            modifier.AddedTraits.Add("Arcane");
            modifier.AddedValidSkills.Add(Skill.Arcane);
            modifier.RestrictedSkills.Add(Skill.Arcane);
            
            return modifier;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();
            
            // Playable on any combat check.
            if (_contexts.CheckContext is { IsCombatValid: true }
                && _contexts.CurrentResolvable is CheckResolvable { HasCombat: true }
                && _contexts.CheckContext?.StagedCardTypes.Count(t => t == card.Data.cardType) == 0)
            {
                actions.Add(new PlayCardAction(card, ActionType.Banish));
            }

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
                OnSuccess = () => card.Owner.Reload(card),
                OnFailure = () => card.Owner.Discard(card)
            };

            return CardUtils.CreateDefaultRecoveryResolvable(resolvable, _gameServices);
        }
    }
}
