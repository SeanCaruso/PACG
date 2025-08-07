using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PACG.Gameplay
{
    public class GameFlowManager
    {
        // GameFlowManager is the sole arbitrator of resolvables and the order in which they're handled.
        private readonly Queue<IResolvable> _resolvableQueue = new(); // Base FIFO queue
        private readonly Stack<IResolvable> _interruptStack = new(); // These take priority over the queue.

        public bool HasResolvables => _resolvableQueue.Count > 0 || _interruptStack.Count > 0;

        public void QueueResolvable(IResolvable resolvable) => _resolvableQueue.Enqueue(resolvable);
        public void QueueInterrupt(IResolvable resolvable) => _interruptStack.Push(resolvable);

        private IResolvable GetNextResolvable() => _interruptStack.Count > 0 ? _interruptStack.Pop() : _resolvableQueue.Dequeue();

        // Other members
        private readonly Queue<IProcessor> _processorQueue = new();
        private readonly ContextManager _contexts;
        private bool _isProcessing = false;

        // Dependency for creating new processors
        private readonly LogicRegistry _logicRegistry;

        public GameFlowManager(ContextManager contextManager, LogicRegistry logicRegistry)
        {
            _contexts = contextManager;
            _logicRegistry = logicRegistry;
        }


        public void QueueProcessor(IProcessor processor)
        {
            _processorQueue.Enqueue(processor);
        }

        public void QueueProcessors(IEnumerable<IProcessor> processors)
        {
            foreach (var processor in processors)
            {
                _processorQueue.Enqueue(processor);
            }
        }

        private bool ShouldContinueProcessing => _processorQueue.Count > 0 || HasResolvables;

        public void Process()
        {
            if (_isProcessing) return;
            _isProcessing = true;

            while (ShouldContinueProcessing)
            {
                if (!ExecuteNextStep()) break;
            }

            _isProcessing = false;
        }

        private bool ExecuteNextStep()
        {
            if (_processorQueue.Count > 0) return ExecuteProcessor();
            if (HasResolvables)
            {
                var resolvable = GetNextResolvable();
                QueueProcessorFor(resolvable);
                return true;
            }
            return false; // All done!
        }

        private bool ExecuteProcessor()
        {
            var processor = _processorQueue.Dequeue();
            Debug.Log($"[GameFlowManager] Executing: {processor.GetType().Name}");
            processor.Execute();

            // If a processor sets up a resolution context, it's asking for user input.
            // We must pause execution and wait for the player to act.
            if (_contexts.ResolutionContext != null)
            {
                Debug.Log($"[GameFlowManager] Pausing for user input...");
                return false;
            }

            return true; // Continue processing
        }

        private void QueueProcessorFor(IResolvable resolvable)
        {
            // Based on the type of resolvable that just finished, queue the appropriate next step.
            if (resolvable is CombatResolvable combat)
            {
                QueueProcessor(new CheckProcessor(combat, _contexts, _logicRegistry, this));
            }
            else if (resolvable is RerollResolvable reroll)
            {
                // Reroll is just another check.
                QueueProcessor(new CheckProcessor(reroll, _contexts, _logicRegistry, this));
            }
            else if (resolvable is DamageResolvable damage)
            {
                QueueProcessor(new DamageProcessor(damage));
            }
        }
    }
}
