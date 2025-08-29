using PACG.Core;
using UnityEngine;

namespace PACG.Gameplay
{
    public class DireWolfLogic : CardLogicBase
    {
        public override bool CanEvade => false;
        
        // Dependency injection
        private readonly ContextManager _contexts;
        
        public DireWolfLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        public override void OnEncounter()
        {
            _contexts.EncounterContext?.ResolvableModifiers.Add(ModifyDamageResolvable);
        }

        private static void ModifyDamageResolvable(IResolvable resolvable)
        {
            if (resolvable is not DamageResolvable damageResolvable) return;

            var damageIncrease = DiceUtils.Roll(4);
            Debug.Log($"Dire Wolf increased damage by {damageIncrease}!");
            damageResolvable.Amount += damageIncrease;
        }
    }
}
