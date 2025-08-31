using System.Collections.Generic;
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

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();

            // Playable for +2d4 on any combat check.
            if (_contexts.CheckContext is not { IsCombatValid: true }
                || _contexts.CurrentResolvable is not CheckResolvable { HasCombat: true }
                || _contexts.CurrentResolvable.IsCardTypeStaged(card.CardType)) return actions;
            var modifier = new CheckModifier(card)
            {
                AddedDice = new List<int> { 4, 4 },
                AddedTraits = new List<string> { "Attack", "Force", "Magic" },
                RestrictedCategory = CheckCategory.Combat
            };

            // Also adds Arcane skill for the owner.
            if (card.Owner == _contexts.CheckContext.Resolvable.Character)
            {
                modifier.AddedTraits.Add("Arcane");
                modifier.AddedValidSkills.Add(Skill.Arcane);
                modifier.RestrictedSkills.Add(Skill.Arcane);
            }

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
