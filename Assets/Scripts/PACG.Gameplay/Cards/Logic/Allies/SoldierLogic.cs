using System.Collections.Generic;
using System.Linq;

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

        public override CheckModifier GetCheckModifier(IStagedAction action)
        {
            // Recharge for +1d4 on a local Strength or Melee check.
            if (action.ActionType != PF.ActionType.Recharge) return null;
            
            var modifier = new CheckModifier(action.Card);
            modifier.RequiredTraits.AddRange(new[] { "Strength", "Melee" });
            modifier.AddedDice.Add(4);
            return modifier;
        }

        public override void OnCommit(IStagedAction action)
        {
            // Discard to explore - +1d4 on Strength and Melee checks.
            if (action.ActionType != PF.ActionType.Discard) return;
            
            _contexts.TurnContext.AddExploreEffect(new SkillBonusExploreEffect(
                1,
                4,
                0,
                false,
                PF.Skill.Strength, PF.Skill.Melee)
            );
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();
            
            if (CanRecharge(card))
                actions.Add(new PlayCardAction(card, PF.ActionType.Recharge));

            if (_contexts.IsExplorePossible && card.Owner == _contexts.TurnContext.Character)
            {
                actions.Add(new ExploreAction(card, PF.ActionType.Discard));
            }

            return actions;
        }

        // Can recharge on a local Strength or Melee check.
        private bool CanRecharge(CardInstance card) => 
            _contexts.CheckContext != null &&
            _contexts.CurrentResolvable is CheckResolvable &&
            _contexts.CheckContext.IsLocal(card.Owner) &&
            !_contexts.CheckContext.StagedCardTypes.Contains(PF.CardType.Ally) &&
            _contexts.CheckContext.Invokes("Strength", "Melee");
    }
}
