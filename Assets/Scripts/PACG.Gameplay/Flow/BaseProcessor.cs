namespace PACG.Gameplay
{
    public abstract class BaseProcessor : IProcessor
    {
        private readonly GameFlowManager _gameFlow;

        protected BaseProcessor(GameServices gameServices)
        {
            _gameFlow = gameServices.GameFlow;
        }

        public void Execute()
        {
            // Call custom processor logic.
            OnExecute();

            // Automatically complete the current phase.
            _gameFlow.CompleteCurrentPhase();
        }

        /// <summary>
        /// Sub-processor-specific functionality. GameFlowManager.CompleteCurrentPhase is handled by BaseProcessor.
        /// </summary>
        protected abstract void OnExecute();
    }
}
