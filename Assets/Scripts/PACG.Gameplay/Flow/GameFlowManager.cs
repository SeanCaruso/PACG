using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class PhaseQueue
    {
        private List<IProcessor> Processors { get; } = new();
        public string Name { get; }

        public int Count => Processors.Count;
        public void Enqueue(IProcessor processor) => Processors.Add(processor);
        public void Interrupt(IProcessor processor) => Processors.Insert(0, processor);

        public IProcessor Dequeue()
        {
            if (Processors.Count == 0) return null;

            var first = Processors[0];
            Processors.Remove(first);
            return first;
        }

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
        public void QueueNextProcessor(IProcessor processor)
        {
            if (_queueStack.Count == 0)
                _queueStack.Push(new PhaseQueue($"{processor.GetType().Name}"));

            Current.Enqueue(processor);
        }

        /// <summary>
        /// Adds a processor to the FRONT of the current phase queue to be processed next.
        /// </summary>
        /// <param name="processor"></param>
        public void Interrupt(IProcessor processor)
        {
            if (_queueStack.Count == 0)
                _queueStack.Push(new PhaseQueue($"{processor.GetType().Name}"));
            
            Current.Interrupt(processor);
        }

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
            Debug.Log($"[{GetType().Name}] StartPhase called with {phaseProcessor}");

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

        public void AbortPhase()
        {
            if (_queueStack.Count > 0)
                _queueStack.Pop();
        }
    }
}
