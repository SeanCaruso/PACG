using UnityEngine;

namespace PACG.Gameplay
{
    public class Check_DamageProcessor : BaseProcessor
    {
        private readonly ContextManager _contexts;

        public Check_DamageProcessor(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        protected override void OnExecute()
        {
            var check = _contexts.CheckContext;

            if (check.CheckResult.WasSuccess)
            {
                Debug.Log($"Rolled {check.CheckResult.FinalRollTotal} vs. {check.CheckResult.DC} - Success!");
            }
            else
            {
                DamageResolvable damageResolvable = new(check.Resolvable.Character, -check.CheckResult.MarginOfSuccess);
                _contexts.NewResolvable(damageResolvable);
                Debug.Log($"Rolled {check.CheckResult.FinalRollTotal} vs. {check.CheckResult.DC} - Take {damageResolvable.Amount} damage!");
            }
        }
    }
}
