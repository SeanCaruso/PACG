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
            var encounterLogic = _logic.GetEncounterLogic(_contexts.EncounterContext.EncounteredCard);
            var resolvables = encounterLogic?.Execute(EncounterPhase.AttemptChecks) ?? new();

            if (resolvables.Any())
            {
                // Set up the resolution context. This is the signal for the game to pause.
                _contexts.NewResolution(new(resolvables.First()));
            }
        }
    }
}
