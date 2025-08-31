using System.Collections.Generic;
using System.Linq;
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
            // Can recharge for +1d4 on a local check against a spell.
            if (_contexts.CheckContext?.Resolvable.Card is CardInstance checkCard
                && checkCard.Data.cardType == CardType.Spell
                && !_contexts.CheckContext.Resolvable.IsCardTypeStaged(CardType.Ally))
            {
                var modifier = new CheckModifier(card) { AddedDice = new[] { 4 }.ToList() };
                return new List<IStagedAction>{ new PlayCardAction(card, ActionType.Recharge, modifier) };
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
