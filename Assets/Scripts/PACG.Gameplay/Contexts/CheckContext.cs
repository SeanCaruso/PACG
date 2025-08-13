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
        public IReadOnlyList<PF.Skill> BaseValidSkills { get; }

        public PlayerCharacter Character => Resolvable.Character;

        public CheckContext(ICheckResolvable resolvable)
        {
            Resolvable = resolvable;
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
            if (!action.IsFreely && _stagedCardTypes.Contains(action.Card.Data.cardType))
            {
                Debug.LogWarning($"{action.Card.Data.cardName} staged a duplicate type - was this intended?");
                return false;
            }

            return true;
        }

        public void StageCardTypeIfNeeded(IStagedAction action)
        {
            if (!action.IsFreely) _stagedCardTypes.Add(action.Card.Data.cardType);
        }

        public void ClearStagedTypes() => _stagedCardTypes.Clear();

        // =====================================================================================
        // CHECK SKILL DETERMINATION
        // =====================================================================================
        private readonly Dictionary<CardInstance, List<PF.Skill>> stagedSkillAdditions = new();
        private readonly Dictionary<CardInstance, List<PF.Skill>> stagedSkillRestrictions = new();

        public void AddValidSkills(CardInstance card, params PF.Skill[] skills)
        {
            if (stagedSkillAdditions.ContainsKey(card))
                stagedSkillAdditions[card].AddRange(skills);
            else
                stagedSkillAdditions.Add(card, new(skills));
        }
        public void RestrictValidSkills(CardInstance card, params PF.Skill[] skills)
        {
            if (stagedSkillRestrictions.ContainsKey(card))
                stagedSkillRestrictions[card].AddRange(skills);
            else
                stagedSkillRestrictions.Add(card, new(skills));
        }
        public void UndoSkillModification(CardInstance source)
        {
            stagedSkillAdditions.Remove(source);
            stagedSkillRestrictions.Remove(source);
        }

        public bool CanPlayCardWithSkills(params PF.Skill[] skills)
        {
            var validSkills = skills.ToList();

            // See if we have any skills left after getting the intersection of currently required skills.
            foreach (var requiredSkills in stagedSkillRestrictions.Values)
                validSkills = validSkills.Intersect(requiredSkills).ToList();

            return validSkills.Count > 0;
        }

        public List<PF.Skill> GetCurrentValidSkills()
        {
            List<PF.Skill> skills = new(BaseValidSkills);

            // Apply all additions.
            foreach (var addedSkills in stagedSkillAdditions.Values)
                skills.AddRange(addedSkills.Except(skills));

            // Apply all restrictions.
            foreach (var restrictedSkills in stagedSkillRestrictions.Values)
                skills = skills.Intersect(restrictedSkills).ToList();

            return skills;
        }

        // =====================================================================================
        // CHECK RESULTS ENCAPSULATION
        // =====================================================================================
        private readonly List<string> _traits = new();
        private readonly DicePool _dicePool = new();

        public PF.Skill UsedSkill { get; set; }
        public IReadOnlyList<string> Traits => _traits;
        public DicePool DicePool => _dicePool;
        public int BlessingCount { get; set; }
        public CheckResult CheckResult { get; set; }

        // Public methods to control state changes
        public void AddTraits(params string[] traits) => _traits.AddRange(traits);
        public void AddToDicePool(int count, int sides, int bonus = 0) => _dicePool.AddDice(count, sides, bonus);

        // --- Custom Data ---
        public Dictionary<string, object> ContextData { get; } = new();
    }
}
