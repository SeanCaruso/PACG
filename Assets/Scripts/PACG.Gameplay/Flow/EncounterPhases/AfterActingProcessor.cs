namespace PACG.Gameplay
{
    public class AfterActingProcessor : BaseProcessor
    {
        private readonly ContextManager _contexts;

        public AfterActingProcessor(GameServices gameServices)
            : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        protected override void OnExecute()
        {
            if (_contexts.EncounterContext == null) return;
            
            _contexts.EncounterContext.CurrentPhase = EncounterPhase.AfterActing;

            if (_contexts.EncounterContext.IgnoreAfterActingPowers) return;
            
            var resolvable = _contexts.EncounterContext.Card.GetAfterActingResolvable();
            if (resolvable != null)
                _contexts.NewResolvable(resolvable);
        }
    }
}
