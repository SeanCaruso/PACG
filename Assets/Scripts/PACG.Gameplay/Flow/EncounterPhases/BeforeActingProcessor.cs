
namespace PACG.Gameplay
{
    public class BeforeActingProcessor : BaseProcessor
    {
        private readonly ContextManager _contexts;

        public BeforeActingProcessor(GameServices gameServices)
            : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        protected override void OnExecute()
        {
            if (_contexts.EncounterContext == null) return;
            
            _contexts.EncounterContext.CurrentPhase = EncounterPhase.BeforeActing;
            
            var resolvables = _contexts.EncounterContext.Card.GetBeforeActingResolvables();
            // TODO: Handle multiple resolvables
            if (resolvables.Count > 0) _contexts.NewResolvable(resolvables[0]);
        }
    }
}
