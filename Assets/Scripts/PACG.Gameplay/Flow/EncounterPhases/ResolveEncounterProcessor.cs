using UnityEngine;

namespace PACG.Gameplay
{
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
            // TODO
            _contexts.EndEncounter();
        }
    }
}
