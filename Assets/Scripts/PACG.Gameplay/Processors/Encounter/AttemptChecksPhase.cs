using System.Linq;
using UnityEngine;

namespace PACG.Gameplay
{
    public class AttemptChecksPhase : IProcessor
    {
        private readonly ContextManager _contexts;
        private readonly LogicRegistry _logic;

        public AttemptChecksPhase(ContextManager contextManager, LogicRegistry logicRegistry)
        {
            _contexts = contextManager;
            _logic = logicRegistry;
        }

        public void Execute()
        {
        }
    }
}
