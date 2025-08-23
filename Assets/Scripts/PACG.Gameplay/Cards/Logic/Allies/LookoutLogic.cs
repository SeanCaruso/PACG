using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public class LookoutLogic : CardLogicBase
    {
        // Dependency injection of services
        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlow;
        private readonly GameServices _gameServices;
        
        public LookoutLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
            _gameServices = gameServices;
        }

        public override CheckModifier GetCheckModifier(IStagedAction action)
        {
            // Recharge for +1d4 on a local Perception check.
            if (_contexts.CheckContext == null) return null;

            var modifier = new CheckModifier(action.Card);
            modifier.RestrictedSkills.Add(PF.Skill.Perception);
            modifier.AddedDice.Add(4);
            return modifier;
        }

        public override void OnCommit(IStagedAction action)
        {
            if (_contexts.CheckContext != null) return;
            switch (action.ActionType)
            {
                // Recharge to examine the top card of your location.
                case PF.ActionType.Recharge:
                    _gameFlow.QueueNextProcessor(new NewResolvableProcessor(
                        new ExamineResolvable(action.Card.Owner.Location, 1), _gameServices));
                    break;
                // Discard to explore. You may evade.
                case PF.ActionType.Discard:
                    _contexts.TurnContext.AddExploreEffect(new EvadeExploreEffect());
                    break;
            }
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();
            
            if (CanRechargeForCheck(card))
                actions.Add(new PlayCardAction(card, PF.ActionType.Recharge));
            
            // Can recharge to examine outside of resolvables or encounters.
            if (_contexts.CurrentResolvable == null &&
                _contexts.EncounterContext == null &&
                card.Owner.Location.Count > 0)
            {
                actions.Add(new PlayCardAction(card, PF.ActionType.Recharge));
            }

            // Can discard to explore.
            if (_contexts.IsExplorePossible && card.Owner == _contexts.TurnContext.Character)
                actions.Add(new ExploreAction(card, PF.ActionType.Discard));

            return actions;
        }

        // Can recharge on a local Perception check.
        private bool CanRechargeForCheck(CardInstance card) => (
            _contexts.CheckContext != null &&
            _contexts.CheckContext.IsLocal(card.Owner) &&
            !_contexts.CheckContext.StagedCardTypes.Contains(PF.CardType.Ally) &&
            _contexts.CheckContext.CanUseSkill(PF.Skill.Perception)
        );
    }
}
