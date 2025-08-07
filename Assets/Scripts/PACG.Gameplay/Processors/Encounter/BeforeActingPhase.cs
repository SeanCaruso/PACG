using UnityEngine;

namespace PACG.Gameplay
{
    public class BeforeActingPhase : IProcessor
    {
        private readonly ContextManager _contexts;
        private readonly LogicRegistry _logic;

        public BeforeActingPhase(ContextManager contextManager, LogicRegistry logicRegistry)
        {
            _contexts = contextManager;
            _logic = logicRegistry;
        }

        public void Execute()
        {
        }
    }
}
