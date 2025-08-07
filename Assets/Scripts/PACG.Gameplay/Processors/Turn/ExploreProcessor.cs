using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    public class ExploreProcessor : IProcessor
    {
        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlowManager;
        private readonly GameServices _gameServices;

        public ExploreProcessor(GameServices gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameFlowManager = gameServices.GameFlow;
            _gameServices = gameServices;
        }

        public void Execute()
        {
            // Set initial availability of turn actions
            _contexts.TurnContext.CanGive = false;
            _contexts.TurnContext.CanMove = false;
            _contexts.TurnContext.CanExplore = false;
            _contexts.TurnContext.CanCloseLocation = false;

            GameEvents.RaiseTurnStateChanged(_contexts.TurnContext);

            CardInstance exploredCard = _contexts.TurnContext.LocationDeck.DrawCard();
            if (exploredCard ==  null )
            {
                Debug.LogError("[ExploreProcessor] Explored card was null!");
                return;
            }

            _contexts.NewEncounter(new(_contexts.TurnContext.CurrentPC, exploredCard));
            _gameFlowManager.QueueProcessor(new EncounterProcessor(_gameServices));
            //_encounterProcessor.InitializeEncounter(_contexts.TurnContext.CurrentPC, exploredCard);
        }
    }
}
