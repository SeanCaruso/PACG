using System.Collections.Generic;
using System.Linq;
using PACG.Core;

namespace PACG.Gameplay
{
    public class ClockworkServantLogic : CardLogicBase
    {
        // Dependency injection of services
        private readonly ContextManager _contexts;
        
        public ClockworkServantLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        public override CheckModifier GetCheckModifier(IStagedAction action)
        {
            // Recharge for +1d6 on a local Intelligence or Craft check.
            if (action.ActionType != ActionType.Recharge) return null;
            
            var modifier = new CheckModifier(action.Card);
            modifier.RequiredTraits.AddRange(new[] { "Intelligence", "Craft" });
            modifier.AddedDice.Add(6);
            return modifier;
        }

        public override void OnCommit(IStagedAction action)
        {
            // Bury or Banish to explore
            if (action.ActionType is not (ActionType.Bury or ActionType.Banish)) return;
            _contexts.TurnContext.AddExploreEffect(new BasicExploreEffect());
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();
            
            if (CanRecharge(card))
                actions.Add(new PlayCardAction(card, ActionType.Recharge));

            if (_contexts.IsExplorePossible && card.Owner == _contexts.TurnContext.Character)
            {
                actions.Add(new ExploreAction(card, ActionType.Bury));
                actions.Add(new ExploreAction(card, ActionType.Banish));
            }

            return actions;
        }

        public override IResolvable GetRecoveryResolvable(CardInstance card)
        {
            // Craft 8 to recharge.
            return new CheckResolvable(card, card.Owner, CardUtils.SkillCheck(8, Skill.Craft));
        }

        // Can recharge on a local Intelligence or Craft check.
        private bool CanRecharge(CardInstance card) => 
            _contexts.CheckContext != null &&
            _contexts.CheckContext.IsLocal(card.Owner) &&
            !_contexts.CheckContext.StagedCardTypes.Contains(card.Data.cardType) &&
            _contexts.CheckContext.Invokes("Intelligence", "Craft");
    }
}
