
namespace PACG.Gameplay
{
    public class AttemptChecksProcessor : BaseProcessor
    {
        private readonly ContextManager _contexts;

        public AttemptChecksProcessor(GameServices gameServices)
            : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        protected override void OnExecute()
        {
            if (_contexts.EncounterContext == null) return;
            
            var resolvable = _contexts.EncounterContext.Card.GetCheckResolvable();
            // TODO: Handle multiple resolvables
            if (resolvable != null) _contexts.NewResolvable(resolvable);
        }
    }
}
