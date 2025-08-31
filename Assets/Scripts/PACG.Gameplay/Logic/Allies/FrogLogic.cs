using System.Collections.Generic;
using PACG.Core;

namespace PACG.Gameplay
{
    public class FrogLogic : CardLogicBase
    {
        // Dependency injection
        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlow;
        private readonly GameServices _gameServices;
        
        public FrogLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
            _gameServices = gameServices;
        }

        public override void OnCommit(IStagedAction action)
        {
            switch (action.ActionType)
            {
                case ActionType.Bury:
                    var resolvable = CardEffects.CreateExploreChoice(_gameServices);
                    var processor = new NewResolvableProcessor(resolvable, _gameServices);
                    _gameFlow.QueueNextProcessor(processor);
                    break;
                case ActionType.Discard:
                    _contexts.TurnContext.AddExploreEffect(new ScourgeImmunityExploreEffect());
                    break;
            }
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();
            
            // The owner can bury to evade an Obstacle or Trap bane.
            if (_contexts.EncounterContext?.CurrentPhase == EncounterPhase.Evasion
                && _contexts.EncounterContext.Character == card.Owner
                && _contexts.EncounterContext.Card.IsBane
                && _contexts.EncounterContext.HasTrait("Obstacle", "Trap"))
            {
                actions.Add(new PlayCardAction(card, ActionType.Bury, null));
            }
            
            // Can discard if the owner can explore.
            if (_contexts.IsExplorePossible && card.Owner == _contexts.TurnContext.Character)
            {
                actions.Add(new ExploreAction(card, ActionType.Discard));
            }

            return actions;
        }
    }
}
