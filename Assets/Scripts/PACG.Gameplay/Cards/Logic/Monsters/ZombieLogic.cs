
using System.Collections.Generic;
using System.Linq;

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
            return card.Data.checkRequirement.checkSteps.Select(check => 
                new CombatResolvable(
                    _contexts.TurnContext.Character,
                    CardUtils.GetDc(check.baseDC, check.adventureLevelMult))
            ).Cast<IResolvable>().ToList();
        }
    }
}
