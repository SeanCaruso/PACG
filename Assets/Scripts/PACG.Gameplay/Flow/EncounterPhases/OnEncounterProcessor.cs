
using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    public class OnEncounterProcessor : BaseProcessor
    {
        private readonly ContextManager _contexts;

        public OnEncounterProcessor(GameServices gameServices)
            : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        protected override void OnExecute()
        {
            var resolvables = _contexts.EncounterContext.Card.GetOnEncounterResolvables();
            // TODO: Handle multiple resolvables
            if (resolvables.Count > 0 ) _contexts.NewResolvable(resolvables[0]);
        }
    }
}
