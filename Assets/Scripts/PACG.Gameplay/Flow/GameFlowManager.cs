using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class GameFlowManager
    {
        private readonly Stack<Queue<IProcessor>> _queueStack = new(); // Hierarchical flow structure
        private Queue<IProcessor> Current => _queueStack.Peek();

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
        /// Adds a processor to the current phase queue.
        /// </summary>
        /// <param name="processor"></param>
        public void QueueNextProcessor(IProcessor processor) => Current.Enqueue(processor);

        /// <summary>
        /// Call this when finished processing.
        /// </summary>
        public void CompleteCurrentPhase()
        {
            // The processor already dequeued itself in Process.
            Debug.Log($"[{GetType().Name}] Processor completed.");
            Process(); // Continue with whatever's next.
        }

        /// <summary>
        /// Entry point for immediately starting a new phase (like a turn).
        /// </summary>
        /// <param name="phaseProcessor">Processor for the new phase</param>
        public void StartPhase(IProcessor phaseProcessor)
        {
            if (phaseProcessor is not IPhaseController)
            {
                Debug.LogWarning($"[{GetType().Name}] StartPhase called with {phaseProcessor}... should it be an IPhaseController?");
            }
            else
            {
                Debug.Log($"[{GetType().Name}] StartPhase called with {phaseProcessor}");
            }
            var queue = new Queue<IProcessor>();
            queue.Enqueue(phaseProcessor);
            _queueStack.Push(queue);
            Process();
        }

        // ========================================================================================

        public void Process()
        {
            // Pause if we have a pending resolvable.
            if (_contexts.CurrentResolvable != null)
            {
                Debug.Log($"[{GetType().Name}] Process paused - found {_contexts.CurrentResolvable}");
                return;
            }

            // Clean up empty queues (pop back to parent phase).
            while (_queueStack.Count > 0 && Current.Count == 0)
            {
                Debug.Log($"[{GetType().Name}] Finished phase queue, popping stack.");
                _queueStack.Pop();
            }

            // Execute the next processor in the current queue.
            if (_queueStack.Count > 0 && Current.Count > 0)
            {
                var processor = Current.Dequeue();
                Debug.Log($"[{GetType().Name}] Executing {processor}");
                processor.Execute();
            }
        }
    }
}
