using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class GameFlowManager
    {
        private readonly Queue<IProcessor> _phaseQueue = new();         // The future to-do list
        private readonly Stack<IProcessor> _phaseStack = new();         // The processors we're processing right now

        // Dependency injection
        private ContextManager _contexts;

        public void Initialize(GameServices gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        // ========================================================================================
        // PUBLIC API FOR PROCESSORS
        // ========================================================================================

        /// <summary>
        /// Adds a processor to the 
        /// </summary>
        /// <param name="processor"></param>
        public void QueueNextPhase(IProcessor processor) => _phaseQueue.Enqueue(processor);

        /// <summary>
        /// Call this when finished processing.
        /// </summary>
        public void CompleteCurrentPhase()
        {
            if (_phaseStack.Count > 0) _phaseStack.Pop();
            Process(); // Continue with whatever's next.
        }

        /// <summary>
        /// Entry point for immediately starting a new phase (like a turn).
        /// </summary>
        /// <param name="phaseProcessor">Processor for the new phase</param>
        public void StartPhase(IProcessor phaseProcessor)
        {
            Debug.Log($"[GFM] StartPhase called with {phaseProcessor.GetType().Name}");
            _phaseStack.Push(phaseProcessor);
            Process();
        }

        // ========================================================================================

        public void Process()
        {
            Debug.Log("[GFM] Process() called");

            // Pause if we have a pending resolvable.
            if (_contexts.CurrentResolvable != null) return;

            // No current phase, see if we have another one to do and set it.
            if (_phaseStack.Count == 0 && _phaseQueue.Count > 0) _phaseStack.Push(_phaseQueue.Dequeue());

            // No sub-processes, continue our current phase. Leave it in the stack until it says it's done.
            if (_phaseStack.Count > 0) _phaseStack.Peek().Execute();
        }
    }
}
