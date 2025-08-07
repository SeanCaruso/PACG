using UnityEngine;

namespace PACG.Gameplay
{
    public class ResolveEncounterPhase : IProcessor
    {
        private readonly ContextManager _contexts;
        private readonly LogicRegistry _logic;

        public ResolveEncounterPhase(ContextManager contextManager, LogicRegistry logicRegistry)
        {
            _contexts = contextManager;
            _logic = logicRegistry;
        }

        public void Execute()
        {
        }
    }
}
