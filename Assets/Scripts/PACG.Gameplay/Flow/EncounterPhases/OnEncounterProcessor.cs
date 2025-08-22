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
            if (_contexts.EncounterContext == null)
            {
                Debug.LogError($"[{GetType().Name}] EncounterContext is null already!");
                return;
            }
            
            _contexts.EncounterContext.CurrentPhase = EncounterPhase.OnEncounter;
            
            var resolvable = _contexts.EncounterContext.Card.GetOnEncounterResolvable();
            // TODO: Handle multiple resolvables
            if (resolvable != null)
                _contexts.NewResolvable(resolvable);
        }
    }
}
