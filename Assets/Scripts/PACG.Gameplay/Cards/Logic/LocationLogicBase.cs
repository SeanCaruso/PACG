using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public abstract class LocationLogicBase : ILogicBase
    {
        private GameServices _gameServices;

        protected LocationLogicBase(GameServices gameServices)
        {
            _gameServices = gameServices;
        }

        // ========================================================================================
        // AT THIS LOCATION
        // ========================================================================================
        public virtual List<IResolvable> GetStartOfTurnResolvables() => new();

        // ========================================================================================
        // CLOSING / WHEN CLOSED
        // ========================================================================================
        public abstract List<IResolvable> GetToCloseResolvables();
        public abstract List<IResolvable> GetWhenClosedResolvables();
    }
}
