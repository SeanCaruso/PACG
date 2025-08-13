using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    public class Turn_ExploreProcessor : IProcessor, IPhaseController
    {
        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlowManager;
        private readonly GameServices _gameServices;

        public Turn_ExploreProcessor(GameServices gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameFlowManager = gameServices.GameFlow;
            _gameServices = gameServices;
        }

        public void Execute()
        {
            Debug.Log("[ExploreProcessor] Starting explore...");

            _contexts.TurnContext.CanGive = false;
            _contexts.TurnContext.CanMove = false;
            _contexts.TurnContext.CanExplore = false;
            _contexts.TurnContext.CanCloseLocation = false;

            GameEvents.RaiseTurnStateChanged();

            CardInstance exploredCard = _contexts.TurnContext.LocationDeck.DrawCard();
            if (exploredCard ==  null )
            {
                Debug.LogError("[ExploreProcessor] Explored card was null!");
                return;
            }

            _gameFlowManager.StartPhase(new EncounterController(_contexts.TurnContext.CurrentPC, exploredCard, _gameServices), $"Explore_{exploredCard.Data.cardName}");

            _gameFlowManager.CompleteCurrentPhase();
        }
    }
}
