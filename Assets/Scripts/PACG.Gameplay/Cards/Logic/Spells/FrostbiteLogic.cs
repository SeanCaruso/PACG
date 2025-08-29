using System.Collections.Generic;
using System.Linq;
using PACG.Core;

namespace PACG.Gameplay
{
    public class FrostbiteLogic : CardLogicBase
    {
        // Dependency injection
        private readonly ContextManager _contexts;
        private readonly GameServices _gameServices;

        public FrostbiteLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameServices = gameServices;
        }

        public override CheckModifier GetCheckModifier(IStagedAction action)
        {
            if (_contexts.CurrentResolvable is not CheckResolvable) return null;

            return new CheckModifier(action.Card)
            {
                RestrictedCategory = CheckCategory.Combat,
                AddedValidSkills = new List<Skill> { Skill.Arcane, Skill.Divine },
                RestrictedSkills = new List<Skill> { Skill.Arcane, Skill.Divine },
                AddedDice = new List<int> { 4, 4 },
                AddedTraits = action.Card.Traits
            };
        }

        public override void OnCommit(IStagedAction action)
        {
            if (_contexts.EncounterContext?.Card.IsBane != true) return;

            _contexts.EncounterContext?.ResolvableModifiers.Add(ModifyDamageResolvable);
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();

            // Playable on the owner's combat check.
            if (_contexts.CheckContext is { IsCombatValid: true }
                && _contexts.CurrentResolvable is CheckResolvable { HasCombat: true } resolvable
                && resolvable.Character == card.Owner
                && !_contexts.CheckContext.Resolvable.IsCardTypeStaged(card.Data.cardType))
            {
                actions.Add(new PlayCardAction(card, ActionType.Banish));
            }

            return actions;
        }

        public override IResolvable GetRecoveryResolvable(CardInstance card)
        {
            if (!card.Owner.IsProficient(card.Data)) return null;

            var checkReq = new CheckRequirement
            {
                mode = CheckMode.Choice
            };
            checkReq.checkSteps.Add(new CheckStep
            {
                category = CheckCategory.Skill,
                baseDC = 8,
                allowedSkills = new[] { Skill.Arcane }.ToList() 
            });
            checkReq.checkSteps.Add(new CheckStep
            {
                category = CheckCategory.Skill,
                baseDC = 10,
                allowedSkills = new[] { Skill.Divine }.ToList() 
            });

            var resolvable = new CheckResolvable(card, card.Owner, checkReq)
            {
                OnSuccess = () => card.Owner.Recharge(card),
                OnFailure = () => card.Owner.Discard(card)
            };

            return CardUtils.CreateDefaultRecoveryResolvable(resolvable, _gameServices);
        }

        private static void ModifyDamageResolvable(IResolvable resolvable)
        {
            if (resolvable is not DamageResolvable damageResolvable) return;

            damageResolvable.Amount -= 1;
        }
    }
}
