
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
            
            var resolvable = _contexts.EncounterContext.Card.GetBeforeActingResolvable();
            if (resolvable != null)
                _contexts.NewResolvable(resolvable);
        }
    }
}
