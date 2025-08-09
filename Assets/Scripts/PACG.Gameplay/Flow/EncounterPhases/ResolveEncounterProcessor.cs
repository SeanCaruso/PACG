using UnityEngine;

namespace PACG.Gameplay
{
    public class ResolveEncounterProcessor : BaseProcessor
    {
        private readonly EncounterContext _context;

        public ResolveEncounterProcessor(EncounterContext context, GameServices gameServices)
            : base(gameServices)
        {
            _context = context;
        }

        protected override void OnExecute()
        {
            // TODO

            _gameServices.Contexts.EndEncounter();
        }
    }
}
