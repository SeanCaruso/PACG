using System.Collections.Generic;
using PACG.Core;
using PACG.Data;

namespace PACG.Gameplay
{
    public class CatLogic : CardLogicBase
    {
        // Dependency injection
        private readonly ContextManager _contexts;
        
        public CatLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        public override CheckModifier GetCheckModifier(IStagedAction action)
        {
            if (action.ActionType != ActionType.Recharge) return null;
            
            var modifier = new CheckModifier(action.Card);
            modifier.AddedDice.Add(4);
            return modifier;
        }

        public override void OnCommit(IStagedAction action)
        {
            if (action.ActionType != ActionType.Discard) return;
            
            // Discard to explore. +1d4 on checks that invoke the Magic trait.
            var exploreEffect = new SkillBonusExploreEffect(1, 4, 0, false);
            exploreEffect.SetTraits("Magic");
            _contexts.TurnContext.AddExploreEffect(exploreEffect);
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            // Can recharge on a local check against a spell.
            if (_contexts.CheckContext?.Resolvable.Card is CardInstance checkCard
                && checkCard.Data.cardType == CardType.Spell
                && !_contexts.CheckContext.Resolvable.IsCardTypeStaged(CardType.Ally))
            {
                return new List<IStagedAction>{ new PlayCardAction(card, ActionType.Recharge) };
            }

            // Discard to explore.
            if (_contexts.IsExplorePossible && card.Owner == _contexts.TurnContext.Character)
            {
                return new List<IStagedAction>{ new ExploreAction(card, ActionType.Discard) };
            }
            
            return new List<IStagedAction>();
        }
    }
}
