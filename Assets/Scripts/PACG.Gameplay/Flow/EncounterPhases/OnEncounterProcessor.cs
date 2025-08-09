
using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    public class OnEncounterProcessor : BaseProcessor
    {
        private readonly EncounterContext _context;

        public OnEncounterProcessor(EncounterContext context, GameServices gameServices)
            : base(gameServices)
        {
            _context = context;
        }

        protected override void OnExecute()
        {
            var resolvables = _context.CardLogic.GetOnEncounterResolvables();
            foreach ( var resolvable in resolvables ) GFM.Interrupt(resolvable);
        }
    }
}
