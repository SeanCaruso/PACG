using UnityEngine;

namespace PACG.Gameplay
{
    public class AfterActingPhase : IProcessor
    {
        private readonly ContextManager _contexts;
        private readonly LogicRegistry _logic;

        public AfterActingPhase(ContextManager contextManager, LogicRegistry logicRegistry)
        {
            _contexts = contextManager;
            _logic = logicRegistry;
        }

        public void Execute()
        {
        }
    }
}
