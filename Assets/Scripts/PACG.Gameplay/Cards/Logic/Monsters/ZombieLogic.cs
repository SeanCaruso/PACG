
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class ZombieLogic : CardLogicBase
    {
        public ZombieLogic(GameServices gameServices) : base(gameServices) { }

        public override List<IResolvable> GetCheckResolvables(CardInstance card)
        {
            var resolvables = new List<IResolvable>();
            foreach (var check in card.Data.checkRequirement.checkSteps)
            {
                resolvables.Add(new CombatResolvable(GameServices.Logic, GameServices.Contexts.TurnContext.CurrentPC, CardUtils.GetDC(check.baseDC, check.adventureLevelMult)));
            }
            return resolvables;
        }
    }
}
