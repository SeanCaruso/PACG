using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PACG.Gameplay
{
    public class GameFlowManager
    {
        private readonly Queue<IProcessor> _phaseQueue = new();         // The future to-do list
        private readonly Stack<IProcessor> _phaseStack = new();         // The processors we're processing right now
        private readonly Stack<IResolvable> _resolvableStack = new();   // Sub-processes (higher priority Resolvables)

        // Dependency injection
        private readonly GameServices _gameServices;

        public GameFlowManager(GameServices gameServices)
        {
            _gameServices = gameServices;
        }

        // ========================================================================================
        // PUBLIC API FOR PROCESSORS
        // ========================================================================================

        /// <summary>
        /// Pause the current processor and run a sub-process.
        /// </summary>
        /// <param name="resolvable">Sub-process to immediately process</param>
        public void Interrupt(IResolvable resolvable)
        {
            _resolvableStack.Push(resolvable);
            Process();
        }

        /// <summary>
        /// Adds a processor to the 
        /// </summary>
        /// <param name="processor"></param>
        public void QueueNextPhase(IProcessor processor) => _phaseStack.Push(processor);

        /// <summary>
        /// Call this when finished processing.
        /// </summary>
        public void CompleteCurrentPhase()
        {
            if (_phaseStack.Count > 0) _phaseStack.Pop();
            Process(); // Continue with whatever's next.
        }

        /// <summary>
        /// Entry point for starting a new phase (like a turn).
        /// </summary>
        /// <param name="phaseProcessor">Processor for the new phase</param>
        public void StartPhase(IProcessor phaseProcessor)
        {
            _phaseStack.Push(phaseProcessor);
            Process();
        }

        // ========================================================================================

        public void Process()
        {
            // Blocking sub-processes are top priority.
            while (_resolvableStack.Count > 0) _resolvableStack.Pop().CreateProcessor(_gameServices).Execute();

            // No current phase, see if we have another one to do and set it.
            if (_phaseStack.Count == 0 && _phaseQueue.Count > 0) _phaseStack.Push(_phaseQueue.Dequeue());

            // No sub-processes, continue our current phase. Leave it in the stack until it says it's done.
            if (_phaseStack.Count > 0) _phaseStack.Peek().Execute();
        }
    }
}
