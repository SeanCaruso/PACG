
using System;
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
        private readonly List<string> _traits = new();

        public SkillBonusExploreEffect(int diceCount, int diceSides, int bonus, bool isForOneCheck, params PF.Skill[] skills)
        {
            _diceCount = diceCount;
            _diceSides = diceSides;
            _bonus = bonus;
            IsForOneCheck = isForOneCheck;
            _skills = skills.ToList();
        }
        
        public void SetTraits(params string[] traits) => _traits.AddRange(traits);
        
        public void ApplyTo(CheckContext check, DicePool dicePool)
        {
            if (!DoesApplyToCheck(check)) return;
            dicePool.AddDice(_diceCount, _diceSides, _bonus);

            var bonus = _bonus > 0 ? $"+{_bonus}" : "";
            Debug.Log($"[{GetType().Name}] Added {_diceCount}d{_diceSides}{bonus} to check.");
        }

        private bool DoesApplyToCheck(CheckContext check)
        {
            if (check == null) return false;

            switch (_skills.Count)
            {
                case > 0 when _traits.Count > 0:
                    throw new NotImplementedException("Found an exploration bonus with both skills and traits!");
                case > 0 when _skills.Contains(check.UsedSkill):
                    return true;
            }

            switch (_traits.Count)
            {
                case > 0 when check.Invokes(_traits.ToArray()):
                case 0 when _skills.Count == 0:
                    return true;
                default:
                    return false;
            }
        }
    }
}
