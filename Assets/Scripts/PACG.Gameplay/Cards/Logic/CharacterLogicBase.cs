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
        public virtual List<IResolvable> GetStartOfTurnResolvables(PlayerCharacter pc) => new();
        public virtual List<IResolvable> GetEndOfTurnResolvables(PlayerCharacter pc) => new();
        // etc.
    }
}
