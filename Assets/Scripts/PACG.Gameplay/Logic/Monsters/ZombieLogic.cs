
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
            var resolvables = new List<IResolvable>();

            if (phase == EncounterPhase.AttemptChecks)
            {
                foreach (var check in Card.Data.checkRequirement.checkSteps)
                {
                    resolvables.Add(new CombatResolvable(Logic, Contexts.TurnContext.CurrentPC, CardUtils.GetDC(check.baseDC, check.adventureLevelMult)));
                }
            }

            return resolvables;
        }
    }
}
