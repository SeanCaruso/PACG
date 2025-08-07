using UnityEngine;

namespace PACG.Gameplay
{
    /// <summary>
    /// Convenience class that contains all of the commonly-used services.
    /// </summary>
    public class GameServices
    {
        public ActionStagingManager ASM { get; }
        public CardManager Cards {  get; }
        public ContextManager Contexts { get; }
        public GameFlowManager GameFlow {  get; }
        public LogicRegistry Logic { get; }

        public GameServices(
            ActionStagingManager actionStagingManager,
            CardManager cardManager,
            ContextManager contextManager,
            GameFlowManager gameFlowManager,
            LogicRegistry logicRegistry)
        {
            ASM = actionStagingManager;
            Cards = cardManager;
            Contexts = contextManager;
            GameFlow = gameFlowManager;
            Logic = logicRegistry;
        }
    }
}
