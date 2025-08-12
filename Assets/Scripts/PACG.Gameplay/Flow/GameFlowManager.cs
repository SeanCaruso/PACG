using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class PhaseQueue
    {
        public Queue<IProcessor> Processors { get; } = new();
        public string Name { get; } = "";

        public int Count => Processors.Count;
        public void Enqueue(IProcessor processor) => Processors.Enqueue(processor);
        public IProcessor Dequeue() => Processors.Dequeue();

        public PhaseQueue(string name)
        {
            Name = name;
        }
    }

    public class GameFlowManager
    {
        private readonly Stack<PhaseQueue> _queueStack = new(); // Hierarchical flow structure
        private PhaseQueue Current => _queueStack.Peek();

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
        /// <param name="name">Name for the queue</param>
        public void StartPhase(IProcessor phaseProcessor, string name)
        {
            if (phaseProcessor is not IPhaseController)
            {
                Debug.LogWarning($"[{GetType().Name}] StartPhase called with {phaseProcessor}... should it be an IPhaseController?");
            }
            else
            {
                Debug.Log($"[{GetType().Name}] StartPhase called with {phaseProcessor}");
            }
            var queue = new PhaseQueue(name);
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
                Debug.Log($"[{GetType().Name}] Finished phase queue {Current.Name}, popping stack.");
                _queueStack.Pop();
            }

            // Execute the next processor in the current queue.
            if (_queueStack.Count > 0 && Current.Count > 0)
            {
                var processor = Current.Dequeue();
                Debug.Log($"[{GetType().Name}] Executing phase {Current.Name} processor: {processor}");
                processor.Execute();
            }
        }
    }
}
