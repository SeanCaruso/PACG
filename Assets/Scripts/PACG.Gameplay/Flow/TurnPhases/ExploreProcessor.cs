using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    public class ExploreProcessor : IProcessor
    {
        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlowManager;
        private readonly GameServices _gameServices;
        private readonly LogicRegistry _logic;

        public ExploreProcessor(GameServices gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameFlowManager = gameServices.GameFlow;
            _gameServices = gameServices;
            _logic = gameServices.Logic;
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

            var cardLogic = _logic.GetCardLogic(exploredCard);
            if (cardLogic == null )
            {
                Debug.LogError("[ExploreProcessor] Explored card logic was null!");
                return;
            }

            _gameFlowManager.StartPhase(new EncounterController(_contexts.TurnContext.CurrentPC, cardLogic, _gameServices));

            _gameFlowManager.CompleteCurrentPhase();
        }
    }
}
