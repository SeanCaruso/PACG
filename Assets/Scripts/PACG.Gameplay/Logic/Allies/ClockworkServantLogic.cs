using System.Collections.Generic;
using System.Linq;
using PACG.Core;

namespace PACG.Gameplay
{
    public class ClockworkServantLogic : CardLogicBase
    {
        // Dependency injection
        private readonly ContextManager _contexts;
        private readonly GameServices _gameServices;
        
        public ClockworkServantLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameServices = gameServices;
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

            // Recharge for +1d6 on a local Intelligence or Craft check.
            if (CanRecharge(card))
            {
                var modifier = new CheckModifier(card)
                {
                    RequiredTraits = new[] { "Intelligence", "Craft" }.ToList(),
                    AddedDice = new[] { 6 }.ToList()
                };
                actions.Add(new PlayCardAction(card, ActionType.Recharge, modifier));
            }

            if (!_contexts.IsExplorePossible || card.Owner != _contexts.TurnContext.Character) return actions;
            
            actions.Add(new ExploreAction(card, ActionType.Bury));
            actions.Add(new ExploreAction(card, ActionType.Banish));

            return actions;
        }

        public override IResolvable GetRecoveryResolvable(CardInstance card)
        {
            // Craft 8 to recharge.
            var resolvable = new CheckResolvable(
                card,
                card.Owner,
                CardUtils.SkillCheck(8, Skill.Craft))
            {
                OnSuccess = () => card.Owner.Recharge(card),
                OnFailure = () => card.Owner.Banish(card, true)
            };

            return CardUtils.CreateDefaultRecoveryResolvable(resolvable, _gameServices);
        }

        // Can recharge on a local Intelligence or Craft check.
        private bool CanRecharge(CardInstance card) => 
            _contexts.CheckContext != null
            && _contexts.CheckContext.IsLocal(card.Owner)
            && !_contexts.CheckContext.Resolvable.IsCardTypeStaged(card.Data.cardType)
            && _contexts.CheckContext.Invokes("Intelligence", "Craft");
    }
}
