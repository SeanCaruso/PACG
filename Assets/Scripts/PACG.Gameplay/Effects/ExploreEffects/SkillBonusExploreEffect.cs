
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PACG.Gameplay
{
    public class SkillBonusExploreEffect : IExploreEffect
    {
        private readonly int _diceCount;
        private readonly int _diceSides;
        private readonly int _bonus;
        public bool IsForOneCheck { get; }
        private readonly List<PF.Skill> _skills;

        public SkillBonusExploreEffect(int diceCount, int diceSides, int bonus, bool isForOneCheck, params PF.Skill[] skills)
        {
            _diceCount = diceCount;
            _diceSides = diceSides;
            _bonus = bonus;
            IsForOneCheck = isForOneCheck;
            _skills = skills.ToList();
        }
        
        public void ApplyToCheck(CheckContext check)
        {
            if (!DoesApplyToCheck(check)) return;
            check.AddToDicePool(_diceCount, _diceSides, _bonus);

            var bonus = _bonus > 0 ? $"+{_bonus}" : "";
            Debug.Log($"[{GetType().Name}] Added {_diceCount}d{_diceSides}{bonus} to check.");
        }

        public bool DoesApplyToCheck(CheckContext check)
        {
            return check != null && (!_skills.Any() || _skills.Contains(check.UsedSkill));
        }
    }
}
