
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
    }
}
