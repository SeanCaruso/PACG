
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    [EncounterLogicFor("Zombie")]
    public class ZombieLogic : CardLogicBase, IEncounterLogic
    {
        public ZombieLogic(ContextManager contextManager, LogicRegistry logicRegistry) : base(contextManager, logicRegistry) { }

        public List<IResolvable> Execute(EncounterPhase phase)
        {
            if (phase == EncounterPhase.AttemptChecks)
            {
                return new List<IResolvable> { new CombatResolvable(Logic, Contexts.TurnContext.CurrentPC, Contexts.CheckContext.TotalDC) };
            }

            return new();
        }
    }
}
