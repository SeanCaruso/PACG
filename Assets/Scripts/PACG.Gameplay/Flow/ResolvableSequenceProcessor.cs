using UnityEngine;

namespace PACG.Gameplay
{
    /// <summary>
    /// Processor used for when a resolvable needs to transition to another resolvable (e.g. activated player powers).
    /// </summary>
    public class ResolvableSequenceProcessor : BaseProcessor
    {
        private readonly IResolvable _nextResolvable;

        private readonly ContextManager _contexts;

        public ResolvableSequenceProcessor(IResolvable nextResolvable, GameServices gameServices) : base(gameServices)
        {
            _nextResolvable = nextResolvable;

            _contexts = gameServices.Contexts;
        }

        protected override void OnExecute() 
        {
            Debug.Log($"[ResolvableSequenceProcessor] Creating next resolvable: {_nextResolvable}");
            _contexts.NewResolvable(_nextResolvable);
        }
    }
}
