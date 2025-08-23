namespace PACG.Gameplay
{
    /// <summary>
    /// Processor that gets any Resolvables when resolving an encounter.
    /// Note that Encounter_EndEncounterProcessor handles the actual cleanup, including banishing/acquiring/shuffling
    /// cards to the appropriate locations.
    /// </summary>
    public class ResolveEncounterProcessor : BaseProcessor
    {
        private readonly ContextManager _contexts;

        public ResolveEncounterProcessor(GameServices gameServices)
            : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        protected override void OnExecute()
        {
            if (_contexts.EncounterContext == null) return;
            
            var encounteredCard = _contexts.EncounterContext.Card;
            var resolvable = encounteredCard.Logic.GetResolveEncounterResolvable(encounteredCard);
            if (resolvable != null)
            {
                _contexts.NewResolvable(resolvable);
            }
        }
    }
}
