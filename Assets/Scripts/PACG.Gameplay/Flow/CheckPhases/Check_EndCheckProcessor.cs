
namespace PACG.Gameplay
{
    /// <summary>
    /// This doesn't correspond to an actual game phase, but handles check cleanup and result storing.
    /// </summary>
    public class Check_EndCheckProcessor : BaseProcessor
    {
        private readonly ContextManager _contexts;

        public Check_EndCheckProcessor(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        protected override void OnExecute()
        {
            if (_contexts.CheckContext?.CheckResult == null) return;
            
            // If we have any defined success/fail callbacks, invoke them.
            if (_contexts.CheckContext.CheckResult.WasSuccess)
                _contexts.CheckContext.Resolvable?.OnSuccess?.Invoke();
            else
                _contexts.CheckContext.Resolvable?.OnFailure?.Invoke();
            
            // If we're in an encounter, store the check result for later processing.
            if (_contexts.EncounterContext != null)
            {
                _contexts.EncounterContext.CheckResult = _contexts.CheckContext.CheckResult;
            }

            _contexts.EndCheck();
        }
    }
}
