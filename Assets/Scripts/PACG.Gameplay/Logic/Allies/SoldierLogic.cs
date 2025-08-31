using System.Collections.Generic;
using System.Linq;
using PACG.Core;
using PACG.Data;

namespace PACG.Gameplay
{
    public class SoldierLogic : CardLogicBase
    {
        // Dependency injection of services
        private readonly ContextManager _contexts;

        public SoldierLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        public override void OnCommit(IStagedAction action)
        {
            // Discard to explore - +1d4 on Strength and Melee checks.
            if (action.ActionType != ActionType.Discard) return;

            _contexts.TurnContext.AddExploreEffect(new SkillBonusExploreEffect(
                1,
                4,
                0,
                false,
                Skill.Strength, Skill.Melee)
            );
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();

            // Recharge for +1d4 on a local Strength or Melee check.
            if (CanRecharge(card))
            {
                var modifier = new CheckModifier(card)
                {
                    RequiredTraits = new[] { "Strength", "Melee" }.ToList(),
                    AddedDice = new[] { 4 }.ToList()
                };
                actions.Add(new PlayCardAction(card, ActionType.Recharge, modifier));
            }

            if (_contexts.IsExplorePossible && card.Owner == _contexts.TurnContext.Character)
            {
                actions.Add(new ExploreAction(card, ActionType.Discard));
            }

            return actions;
        }

        // Can recharge on a local Strength or Melee check.
        private bool CanRecharge(CardInstance card) =>
            _contexts.CheckContext != null &&
            _contexts.CurrentResolvable is CheckResolvable &&
            _contexts.CheckContext.IsLocal(card.Owner) &&
            !_contexts.CheckContext.Resolvable.IsCardTypeStaged(CardType.Ally) &&
            _contexts.CheckContext.Invokes("Strength", "Melee");
    }
}
