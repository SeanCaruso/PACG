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
            var resolvable = _contexts.TurnContext.Character.GetEndOfTurnResolvable();
            if (resolvable != null)
                _contexts.NewResolvable(resolvable);
        }
    }
}
