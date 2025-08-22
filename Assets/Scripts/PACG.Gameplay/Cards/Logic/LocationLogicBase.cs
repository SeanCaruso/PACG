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
        public virtual IResolvable GetStartOfTurnResolvables() => null;

        // ========================================================================================
        // CLOSING / WHEN CLOSED
        // ========================================================================================
        public abstract IResolvable GetToCloseResolvables();
        public abstract IResolvable GetWhenClosedResolvable();
    }
}
