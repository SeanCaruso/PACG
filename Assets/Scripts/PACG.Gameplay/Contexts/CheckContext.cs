using System;
using System.Collections.Generic;
using System.Linq;
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
        // --- Immutable Initial State ---
        // These are set once and should never change.
        public ICheckResolvable Resolvable { get; }
        private readonly List<PF.Skill> _baseValidSkills;

        public PlayerCharacter Character => Resolvable.Character;

        public List<IExploreEffect> ExploreEffects { get; set; } = new();

        public CheckContext(ICheckResolvable resolvable)
        {
            Resolvable = resolvable;
            _baseValidSkills = resolvable.Skills.ToList();
        }

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

        public void ClearStagedTypes() => _stagedCardTypes.Clear();

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
        }
        public void RestrictValidSkills(CardInstance card, params PF.Skill[] skills)
        {
            if (_stagedSkillRestrictions.TryGetValue(card, out var restriction))
                restriction.AddRange(skills);
            else
                _stagedSkillRestrictions.Add(card, new List<PF.Skill>(skills));
        }
        public void UndoSkillModification(CardInstance source)
        {
            _stagedSkillAdditions.Remove(source);
            _stagedSkillRestrictions.Remove(source);
        }

        /// <summary>
        /// Convenience function to determine if a card with the given skills can be played on the current set of valid
        /// skills for the check.
        /// </summary>
        /// <param name="skills">card skills</param>
        /// <returns>true if the current valid skills contains one or more of the given skills</returns>
        public bool CanUseSkill(params PF.Skill[] skills) => skills.Intersect(GetCurrentValidSkills()).Any();

        private List<PF.Skill> GetCurrentValidSkills()
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
        private readonly List<string> _traits = new();

        public PF.Skill UsedSkill { get; set; }
        public IReadOnlyList<string> Traits => _traits;
        public DicePool DicePool { get; } = new();

        public int BlessingCount { get; set; }
        public CheckResult CheckResult { get; set; }

        // Public methods to control state changes
        public void AddTraits(params string[] traits) => _traits.AddRange(traits);
        public void AddToDicePool(int count, int sides, int bonus = 0) => DicePool.AddDice(count, sides, bonus);

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
    }
}
