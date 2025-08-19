using System;

namespace PACG.Gameplay
{
    /// <summary>
    /// Generic Resolvable that calls a callback when resolved.
    /// </summary>
    public class GenericResolvable : BaseResolvable
    {
        private readonly Action _callback; 
        
        public GenericResolvable(Action callback)
        {
            _callback = callback;
        }
        
        public override void Resolve() => _callback?.Invoke();
    }
}
