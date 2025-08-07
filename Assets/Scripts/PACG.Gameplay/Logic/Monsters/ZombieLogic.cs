
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    [EncounterLogicFor("Zombie")]
    public class ZombieLogic : CardLogicBase, IEncounterLogic
    {
        public ZombieLogic(GameServices gameServices) : base(gameServices) { }

        public void Execute()
        {
            foreach (var check in Card.Data.checkRequirement.checkSteps)
            {
                GameServices.GameFlow.QueueResolvable(new CombatResolvable(GameServices.Logic, GameServices.Contexts.TurnContext.CurrentPC, CardUtils.GetDC(check.baseDC, check.adventureLevelMult)));
            }
        }
    }
}
