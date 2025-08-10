using UnityEngine;

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
            var resolvables = _contexts.EncounterContext.CardLogic.GetCheckResolvables();
            // TODO: Handle multiple resolvables
            if (resolvables.Count > 0) _contexts.NewResolvable(resolvables[0]);
        }
    }
}
