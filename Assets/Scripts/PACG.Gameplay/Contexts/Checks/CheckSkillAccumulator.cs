using System.Collections.Generic;
using System.Linq;
using PACG.SharedAPI;

namespace PACG.Gameplay
{
    public class CheckSkillAccumulator
    {
        private readonly List<PF.Skill> _baseValidSkills = new();
        private readonly Dictionary<CardInstance, List<PF.Skill>> _stagedSkillAdditions = new();
        private readonly Dictionary<CardInstance, List<PF.Skill>> _stagedSkillRestrictions = new();
        
        public CheckSkillAccumulator(CheckResolvable resolvable)
        {
            foreach (var checkStep in resolvable.CheckSteps)
            {
                if (checkStep.category == CheckCategory.Combat)
                {
                    _baseValidSkills.Add(PF.Skill.Strength);
                    _baseValidSkills.Add(PF.Skill.Melee);
                }
                else
                {
                    _baseValidSkills.AddRange(checkStep.allowedSkills);
                }
            }
        }

        public void AddValidSkills(CardInstance card, params PF.Skill[] skills)
        {
            if (skills.Length == 0) return;
            
            if (_stagedSkillAdditions.TryGetValue(card, out var addition))
                addition.AddRange(skills);
            else
                _stagedSkillAdditions.Add(card, new List<PF.Skill>(skills));
            
            DialogEvents.RaiseValidSkillsChanged(GetCurrentValidSkills());
        }
        public void RestrictValidSkills(CardInstance card, params PF.Skill[] skills)
        {
            if (skills.Length == 0) return;
            
            if (_stagedSkillRestrictions.TryGetValue(card, out var restriction))
                restriction.AddRange(skills);
            else
                _stagedSkillRestrictions.Add(card, new List<PF.Skill>(skills));
            
            DialogEvents.RaiseValidSkillsChanged(GetCurrentValidSkills());
        }

        /// <summary>
        /// Convenience function to determine if a card with the given skills can be played on the current set of valid
        /// skills for the check.
        /// </summary>
        /// <param name="skills">card skills</param>
        /// <returns>true if the current valid skills contains one or more of the given skills</returns>
        public bool CanUseSkill(params PF.Skill[] skills) => skills.Intersect(GetCurrentValidSkills()).Any();

        public List<PF.Skill> GetCurrentValidSkills()
        {
            List<PF.Skill> skills = new(_baseValidSkills);

            // Apply all additions.
            foreach (var addedSkills in _stagedSkillAdditions.Values)
                skills.AddRange(addedSkills.Except(skills));

            // Apply all restrictions.
            return _stagedSkillRestrictions.Values.Aggregate(
                skills,
                (current, restrictedSkills) => current.Intersect(restrictedSkills).ToList());
        }
    }
}
