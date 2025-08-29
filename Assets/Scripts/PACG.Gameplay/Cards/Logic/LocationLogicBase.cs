using PACG.Data;

namespace PACG.Gameplay
{
    public abstract class LocationLogicBase : ILogicBase
    {
        // ========================================================================================
        // AT THIS LOCATION
        // ========================================================================================
        public virtual IResolvable GetStartOfTurnResolvable() => null;
        public virtual LocationPower? GetStartOfTurnPower(Location location) => null;
        public virtual IResolvable GetEndOfTurnResolvable() => null;
        public virtual LocationPower? GetEndOfTurnPower(Location location) => null;

        // ========================================================================================
        // CLOSING / WHEN CLOSED
        // ========================================================================================
        public abstract IResolvable GetToCloseResolvable();
        public abstract IResolvable GetWhenClosedResolvable();
    }
}
