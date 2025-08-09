using UnityEngine;

namespace PACG.Gameplay
{
    public class BeforeActingProcessor : BaseProcessor
    {
        private readonly EncounterContext _context;

        public BeforeActingProcessor(EncounterContext context, GameServices gameServices)
            : base(gameServices)
        {
            _context = context;
        }

        protected override void OnExecute()
        {
            var resolvables = _context.CardLogic.GetBeforeActingResolvables();
            foreach (var resolvable in resolvables) GFM.Interrupt(resolvable);
        }
    }
}
