using PACG.Core;
using UnityEngine;

namespace PACG.Gameplay
{
    public class DireWolfLogic : CardLogicBase
    {
        public override bool CanEvade => false;
        
        public DireWolfLogic(GameServices gameServices) : base(gameServices)
        {
        }

        public override void ModifyResolvable(IResolvable resolvable)
        {
            if (resolvable is not DamageResolvable damageResolvable) return;

            var damageIncrease = DiceUtils.Roll(4);
            Debug.Log($"Dire Wolf increased damage by {damageIncrease}!");
            damageResolvable.Amount += damageIncrease;
        }
    }
}
