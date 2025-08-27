using System.Collections.Generic;
using System.Linq;
using PACG.Data;

namespace PACG.Gameplay
{
    public class CampsiteLogic : LocationLogicBase
    {
        // Dependency injections
        private readonly ContextManager _contexts;
        
        public CampsiteLogic(GameServices gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        public override LocationPower? GetEndOfTurnPower(Location location)
        {
            var pc = _contexts.TurnContext.Character;
            
            // At end of turn, you may heal a card (also prompt for Poisoned/Wounded)
            if (pc.Discards.Count == 0
                && !pc.ActiveScourges.Any(s => s is ScourgeType.Poisoned or ScourgeType.Wounded))
            {
                return null;
            }

            return location.LocationData.AtLocationPower;
        }

        public override IResolvable GetToCloseResolvable()
        {
            throw new System.NotImplementedException();
        }

        public override IResolvable GetWhenClosedResolvable() => null;
    }
}
