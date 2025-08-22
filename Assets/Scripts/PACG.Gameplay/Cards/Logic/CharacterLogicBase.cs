using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public abstract class CharacterLogicBase : ILogicBase
    {
        private readonly GameServices _gameServices;

        protected CharacterLogicBase(GameServices gameServices)
        {
            _gameServices = gameServices;
        }

        // Override these as needed
        public virtual IResolvable GetStartOfTurnResolvable(PlayerCharacter pc) => null;

        public virtual IResolvable GetEndOfTurnResolvable(PlayerCharacter pc) => null;
        // etc.
    }
}
