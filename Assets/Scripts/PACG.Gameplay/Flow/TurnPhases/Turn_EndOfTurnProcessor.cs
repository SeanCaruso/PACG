using UnityEngine;

namespace PACG.Gameplay
{
    public class Turn_EndOfTurnProcessor : BaseProcessor
    {
        private readonly ContextManager _contexts;

        public Turn_EndOfTurnProcessor(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        protected override void OnExecute()
        {
            var resolvables = _contexts.TurnContext.CurrentPC.GetEndOfTurnResolvables();
            if (resolvables.Count > 0)
                _contexts.NewResolvable(resolvables[0]);
        }
    }
}
