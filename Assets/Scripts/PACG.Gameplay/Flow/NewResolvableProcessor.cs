using UnityEngine;

namespace PACG.Gameplay
{
    /// <summary>
    /// Processor that creates a new Resolvable when executed.
    /// </summary>
    public class NewResolvableProcessor : BaseProcessor
    {
        private readonly IResolvable _nextResolvable;

        private readonly ContextManager _contexts;

        public NewResolvableProcessor(IResolvable nextResolvable, GameServices gameServices) : base(gameServices)
        {
            _nextResolvable = nextResolvable;

            _contexts = gameServices.Contexts;
        }

        protected override void OnExecute() 
        {
            Debug.Log($"[NewResolvableProcessor] Creating next resolvable: {_nextResolvable}");
            _contexts.NewResolvable(_nextResolvable);
        }
    }
}
