using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using PACG.SharedAPI;
using UnityEngine;


namespace PACG.Gameplay
{
    [Obsolete("The current resolvable is a better indicator of this.")]
    public enum CheckPhase
    {
        PlayCards,
        RollDice,
        SufferDamage
    }

    public class CheckContext
    {
        private TraitAccumulator _traits;
        
        // --- Immutable Initial State ---
        // These are set once and should never change.
        public CheckResolvable Resolvable { get; }
        private readonly List<PF.Skill> _baseValidSkills = new();

        public PlayerCharacter Character => Resolvable.Character;

        public List<IExploreEffect> ExploreEffects { get; set; } = new();

        public CheckContext(CheckResolvable resolvable)
        {
            Resolvable = resolvable;
            foreach (var checkStep in Resolvable.CheckSteps)
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
            
            _traits = new TraitAccumulator(Resolvable);
            
            // Initialize UsedSkill to the PC's best valid skill.
            UsedSkill = Character.GetBestSkill(_baseValidSkills.ToArray()).skill;
        }
        
        // =====================================================================================
        // TRAITS
        // =====================================================================================
        /// <summary>
        /// Adds traits for the given card.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="traits">Traits to add. If empty, card's traits are added.</param>
        public void AddTraits(ICard card, params string[] traits) =>
            _traits?.Add(card, traits.Length > 0 ? traits : card.Traits.ToArray());
        public void RemoveTraits(ICard card) => _traits.Remove(card);
        public IReadOnlyList<string> Traits
        {
            get
            {
                var playedTraits = _traits.Traits.ToList();
                playedTraits.Add(UsedSkill.ToString());
                if (Character.GetAttributeForSkill(UsedSkill) != UsedSkill)
                    playedTraits.Add(Character.GetAttributeForSkill(UsedSkill).ToString());
                return playedTraits;
            }
        }
        
        /// <summary>
        /// Returns whether the check invokes any of the given traits.
        /// A check invokes a trait if the card played to determine the skill has the trait, or if the card
        /// that triggered the check has the trait.
        /// </summary>
        /// <param name="traits"></param>
        /// <returns>true if the check invokes at least one of the given traits</returns>
        public bool Invokes(params string[] traits) =>  Traits.Intersect(traits).Any();

        // =====================================================================================
        // CONTEXT-SPECIFIC ACTION STAGING
        //
        // Note: ActionStagingManager is responsible for the actual actions and cards that have
        //       been staged, but is rule-agnostic. CheckContext contains the rule-specific
        //       logic about which actions *can* be staged during a Check.
        // =====================================================================================
        private readonly HashSet<PF.CardType> _stagedCardTypes = new();
        public IReadOnlyCollection<PF.CardType> StagedCardTypes => _stagedCardTypes;

        public bool CanStageAction(IStagedAction action)
        {
            // Rule: prevent duplicate card types (if not freely playable).
            if (action.IsFreely || !_stagedCardTypes.Contains(action.Card.Data.cardType)) return true;
            
            Debug.LogWarning($"{action.Card.Data.cardName} staged a duplicate type - was this intended?");
            return false;

        }

        public void StageCardTypeIfNeeded(IStagedAction action)
        {
            if (!action.IsFreely) _stagedCardTypes.Add(action.Card.Data.cardType);
        }
        // =====================================================================================
        // CHECK TYPE (COMBAT/SKILL) DETERMINATION
        // =====================================================================================
        private CheckCategory? _checkRestriction;
        private readonly List<CardInstance> _categoryRestrictionCards = new();

        public bool IsCombatValid => Resolvable.HasCombat && _checkRestriction is not CheckCategory.Skill;
        public bool IsSkillValid => Resolvable.HasSkill && _checkRestriction is not CheckCategory.Combat;

        public void RestrictCheckCategory(CardInstance card, CheckCategory category)
        {
            if (_checkRestriction != null && _checkRestriction != category)
            {
                Debug.LogError($"[{GetType().Name}] {card.Name} attempted to restrict invalid check category.");
                return;
            }
            
            _checkRestriction = category;
            _categoryRestrictionCards.Add(card);
            
            DialogEvents.RaiseValidSkillsChanged(GetCurrentValidSkills());
        }

        public void UndoCheckRestriction(CardInstance source)
        {
            _categoryRestrictionCards.Remove(source);
            if (_categoryRestrictionCards.Count == 0) _checkRestriction = null;
            
            DialogEvents.RaiseValidSkillsChanged(GetCurrentValidSkills());
        }

        [CanBeNull]
        private CheckStep GetForcedCheckStep()
        {
            if (_checkRestriction == null) return null;
            return Resolvable.CheckSteps.FirstOrDefault(step => step.category == _checkRestriction);
        }

        public int GetDcForSkill(PF.Skill skill)
        {
            var forcedStep = GetForcedCheckStep();
            if (forcedStep != null)
                return CardUtils.GetDc(forcedStep.baseDC, forcedStep.adventureLevelMult);
            
            var stepWithSkill = Resolvable.CheckSteps.FirstOrDefault(step => step.allowedSkills.Contains(skill));
            if (stepWithSkill != null)
                return CardUtils.GetDc(stepWithSkill.baseDC, stepWithSkill.adventureLevelMult);
            
            Debug.LogError($"[{GetType().Name}] No check step found for skill {skill}");
            return 0;
        }

        public CheckStep GetActiveCheckStep()
        {
            var forcedStep = GetForcedCheckStep();
            if (forcedStep != null) return forcedStep;
            
            var stepWithSkill = Resolvable.CheckSteps.FirstOrDefault(step => step.allowedSkills.Contains(UsedSkill));
            if (stepWithSkill != null) return stepWithSkill;

            return Resolvable.CheckSteps.First();
        }

        public int GetDc() => CardUtils.GetDc(GetActiveCheckStep().baseDC, GetActiveCheckStep().adventureLevelMult);
        
        public bool IsCombatCheck => GetActiveCheckStep().category == CheckCategory.Combat;

        // =====================================================================================
        // CHECK SKILL DETERMINATION
        // =====================================================================================
        private readonly Dictionary<CardInstance, List<PF.Skill>> _stagedSkillAdditions = new();
        private readonly Dictionary<CardInstance, List<PF.Skill>> _stagedSkillRestrictions = new();

        public void AddValidSkills(CardInstance card, params PF.Skill[] skills)
        {
            if (_stagedSkillAdditions.TryGetValue(card, out var addition))
                addition.AddRange(skills);
            else
                _stagedSkillAdditions.Add(card, new List<PF.Skill>(skills));
            
            DialogEvents.RaiseValidSkillsChanged(GetCurrentValidSkills());
        }
        public void RestrictValidSkills(CardInstance card, params PF.Skill[] skills)
        {
            if (_stagedSkillRestrictions.TryGetValue(card, out var restriction))
                restriction.AddRange(skills);
            else
                _stagedSkillRestrictions.Add(card, new List<PF.Skill>(skills));
            
            var newValidSkills = GetCurrentValidSkills();
            if (!newValidSkills.Contains(UsedSkill))
                UsedSkill = Character.GetBestSkill(newValidSkills.ToArray()).skill;
            
            DialogEvents.RaiseValidSkillsChanged(newValidSkills);
        }
        public void UndoSkillModification(CardInstance source)
        {
            _stagedSkillAdditions.Remove(source);
            _stagedSkillRestrictions.Remove(source);
            
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

        // =====================================================================================
        // CHECK RESULTS ENCAPSULATION
        // =====================================================================================

        public PF.Skill UsedSkill { get; set; }
        public int BlessingCount { get; set; }
        public DicePool DicePool { get; set; } = new();
        public CheckResult CheckResult { get; set; }

        private CardInstance _dieOverrideSource;
        public int? DieOverride { get; private set; }

        public void SetDieOverride(CardInstance source, int sides)
        {
            _dieOverrideSource = source;
            DieOverride = sides;
        }

        public void UndoDieOverride(CardInstance source)
        {
            if (_dieOverrideSource != source) return;
            
            _dieOverrideSource = null;
            DieOverride = null;
        }

        // Public methods to control state changes
        //public void AddToDicePool(int count, int sides, int bonus = 0) => DicePool.AddDice(count, sides, bonus);

        // --- Custom Data ---
        public Dictionary<string, object> ContextData { get; } = new();

        // =====================================================================================
        // CONVENIENCE FUNCTIONS
        // =====================================================================================
        /// <summary>
        /// Returns whether the given PC is in the same location as the PC making the check.
        /// </summary>
        /// <param name="pc"></param>
        /// <returns>true if the given PC is local to the check</returns>
        public bool IsLocal(PlayerCharacter pc) => Character.Location == pc.Location;

        public void Reset()
        {
            BlessingCount = 0;
            _stagedCardTypes.Clear();
            _stagedSkillAdditions.Clear();
            _stagedSkillRestrictions.Clear();
            _traits = new TraitAccumulator(Resolvable);
            ContextData.Clear();
        }
    }
}
