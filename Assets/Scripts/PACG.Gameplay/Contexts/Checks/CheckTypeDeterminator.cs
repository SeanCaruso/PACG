using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using PACG.Core;
using UnityEngine;

namespace PACG.Gameplay
{
    public class CheckTypeDeterminator
    {
        private readonly CheckResolvable _resolvable;
        
        private CheckCategory? _checkRestriction;
        private readonly List<CardInstance> _categoryRestrictionCards = new();
        
        public CheckTypeDeterminator(CheckResolvable resolvable)
        {
            _resolvable = resolvable;
        }

        public bool IsCombatValid => _resolvable.HasCombat && _checkRestriction is not CheckCategory.Skill;
        public bool IsSkillValid => _resolvable.HasSkill && _checkRestriction is not CheckCategory.Combat;

        public void RestrictCheckCategory(CardInstance card, CheckCategory category)
        {
            if (_checkRestriction != null && _checkRestriction != category)
            {
                Debug.LogError($"[{GetType().Name}] {card.Name} attempted to restrict invalid check category.");
                return;
            }
            
            _checkRestriction = category;
            _categoryRestrictionCards.Add(card);
        }

        public void UndoCheckRestriction(CardInstance source)
        {
            _categoryRestrictionCards.Remove(source);
            if (_categoryRestrictionCards.Count == 0) _checkRestriction = null;
        }

        [CanBeNull]
        public CheckStep GetForcedCheckStep()
        {
            if (_checkRestriction == null) return null;
            return _resolvable.CheckSteps.FirstOrDefault(step => step.category == _checkRestriction);
        }

        public int GetDcForSkill(Skill skill)
        {
            var forcedStep = GetForcedCheckStep();
            if (forcedStep != null)
                return CardUtils.GetDc(forcedStep.baseDC, forcedStep.adventureLevelMult);
            
            var stepWithSkill = _resolvable.CheckSteps.FirstOrDefault(step => step.allowedSkills.Contains(skill));
            if (stepWithSkill != null)
                return CardUtils.GetDc(stepWithSkill.baseDC, stepWithSkill.adventureLevelMult);
            
            Debug.LogError($"[{GetType().Name}] No check step found for skill {skill}");
            return 0;
        }
    }
}
