using System.Linq;
using PACG.Data;

namespace PACG.Gameplay
{
    public class CampsiteLogic : LocationLogicBase
    {
        // Dependency injections
        private readonly ActionStagingManager _asm;
        private readonly ContextManager _contexts;
        
        public CampsiteLogic(GameServices gameServices)
        {
            _asm = gameServices.ASM;
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

            var healPower = location.LocationData.AtLocationPower;
            healPower.OnActivate = () =>
            {
                _contexts.TurnContext.PerformedLocationPowers.Add(healPower);
                pc.Heal(1);
                _asm.Commit();
            };

            return healPower;
        }

        public override IResolvable GetToCloseResolvable()
        {
            throw new System.NotImplementedException();
        }

        public override IResolvable GetWhenClosedResolvable() => null;
    }
}
