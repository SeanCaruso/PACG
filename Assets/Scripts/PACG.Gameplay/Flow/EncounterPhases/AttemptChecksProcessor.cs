using UnityEngine;

namespace PACG.Gameplay
{
    public class AttemptChecksProcessor : BaseProcessor
    {
        private readonly EncounterContext _context;

        public AttemptChecksProcessor(EncounterContext context, GameServices gameServices)
            : base(gameServices)
        {
            _context = context;
        }

        protected override void OnExecute()
        {
            var resolvables = _context.CardLogic.GetCheckResolvables();
            foreach (var resolvable in resolvables) GFM.Interrupt(resolvable);
        }
    }
}
