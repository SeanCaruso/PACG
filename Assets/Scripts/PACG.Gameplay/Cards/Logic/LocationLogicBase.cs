
namespace PACG.Gameplay
{
    public abstract class LocationLogicBase : ILogicBase
    {
        // Dependency injections
        private readonly ContextManager _contexts;
        private readonly GameServices _gameServices;

        protected LocationLogicBase(GameServices gameServices)
        {
            _contexts = gameServices.Contexts;
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
