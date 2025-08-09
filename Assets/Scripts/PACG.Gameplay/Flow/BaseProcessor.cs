using UnityEngine;

namespace PACG.Gameplay
{
    public abstract class BaseProcessor : IProcessor
    {
        protected readonly GameServices _gameServices;
        protected GameFlowManager GFM => _gameServices.GameFlow;
        protected LogicRegistry Logic => _gameServices.Logic;

        protected BaseProcessor(GameServices gameServices)
        {
            _gameServices = gameServices;
        }

        public void Execute()
        {
            // Call custom processor logic.
            OnExecute();

            // Automatically complete the current phase.
            GFM.CompleteCurrentPhase();
        }

        protected abstract void OnExecute();
    }
}
