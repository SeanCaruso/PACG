
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class ZombieLogic : CardLogicBase
    {
        private readonly ContextManager _contexts;

        public ZombieLogic(GameServices gameServices) : base(gameServices) 
        {
            _contexts = gameServices.Contexts;
        }

        public override List<IResolvable> GetCheckResolvables(CardInstance card)
        {
            var resolvables = new List<IResolvable>();
            foreach (var check in card.Data.checkRequirement.checkSteps)
            {
                resolvables.Add(new CombatResolvable(_contexts.TurnContext.Character, CardUtils.GetDC(check.baseDC, check.adventureLevelMult)));
            }
            return resolvables;
        }
    }
}
