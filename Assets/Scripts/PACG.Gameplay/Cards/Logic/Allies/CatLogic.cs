using System.Collections.Generic;
using System.Linq;

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

        public override void Execute(CardInstance card, IStagedAction action)
        {
            switch (action.ActionType)
            {
                // Recharge for +1d4 on a check.
                case PF.ActionType.Recharge:
                    _contexts.CheckContext.AddToDicePool(1, 4);
                    break;
                // Discard to explore
                case PF.ActionType.Discard:
                    var exploreEffect = new SkillBonusExploreEffect(1, 4, 0, false);
                    exploreEffect.SetTraits("Magic");
                    _contexts.TurnContext.AddExploreEffect(exploreEffect);
                    break;
            }
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            // Can recharge on a local check against a spell.
            if (_contexts.CheckContext?.Resolvable.Card is CardInstance checkCard &&
                checkCard.Data.cardType == PF.CardType.Spell &&
                !_contexts.CheckContext.StagedCardTypes.Contains(PF.CardType.Ally))
            {
                return new List<IStagedAction>{ new PlayCardAction(card, PF.ActionType.Recharge) };
            }

            // Discard to explore.
            if (_contexts.IsExplorePossible && card.Owner == _contexts.TurnContext.Character)
            {
                return new List<IStagedAction>{ new ExploreAction(card, PF.ActionType.Discard) };
            }
            
            return new List<IStagedAction>();
        }
    }
}
