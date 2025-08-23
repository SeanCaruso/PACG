using System;
using System.Collections.Generic;
using System.Linq;
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
        private CheckSkillAccumulator _skills;
        private CheckTypeDeterminator _typeDeterminator;
        private TraitAccumulator _traits;
        
        // --- Immutable Initial State ---
        // These are set once and should never change.
        public CheckResolvable Resolvable { get; }

        public PlayerCharacter Character => Resolvable.Character;

        public List<IExploreEffect> ExploreEffects { get; set; } = new();

        public CheckContext(CheckResolvable resolvable)
        {
            Resolvable = resolvable;
            
            _skills = new CheckSkillAccumulator(resolvable);
            _typeDeterminator = new CheckTypeDeterminator(resolvable);
            _traits = new TraitAccumulator(resolvable);
            
            // Initialize UsedSkill to the PC's best valid skill.
            UsedSkill = Character.GetBestSkill(GetCurrentValidSkills().ToArray()).skill;
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
        // TYPE DETERMINATION PASSTHROUGHS TO CheckTypeDeterminator
        // =====================================================================================
        public bool IsCombatValid => _typeDeterminator.IsCombatValid;
        public bool IsSkillValid => _typeDeterminator.IsSkillValid;

        public void RestrictCheckCategory(CardInstance card, CheckCategory category)
        {
            _typeDeterminator.RestrictCheckCategory(card, category);
        }

        public void UndoCheckRestriction(CardInstance source)
        {
            _typeDeterminator.UndoCheckRestriction(source);
        }

        public int GetDcForSkill(PF.Skill skill) => _typeDeterminator.GetDcForSkill(skill);

        private CheckStep GetActiveCheckStep()
        {
            var forcedStep = _typeDeterminator.GetForcedCheckStep();
            if (forcedStep != null) return forcedStep;
            
            var stepWithSkill = Resolvable.CheckSteps.FirstOrDefault(step => step.allowedSkills.Contains(UsedSkill));
            if (stepWithSkill != null) return stepWithSkill;

            return Resolvable.CheckSteps.First();
        }

        public int GetDc() => CardUtils.GetDc(GetActiveCheckStep().baseDC, GetActiveCheckStep().adventureLevelMult);
        
        public bool IsCombatCheck => GetActiveCheckStep().category == CheckCategory.Combat;

        // =====================================================================================
        // SKILL PASSTHROUGHS TO CheckSkillAccumulator
        // =====================================================================================

        public void AddValidSkills(CardInstance card, params PF.Skill[] skills) => _skills.AddValidSkills(card, skills);
        public void RestrictValidSkills(CardInstance card, params PF.Skill[] skills)
        {
            _skills.RestrictValidSkills(card, skills);
            
            var newValidSkills = GetCurrentValidSkills();
            if (!newValidSkills.Contains(UsedSkill))
                UsedSkill = Character.GetBestSkill(newValidSkills.ToArray()).skill;
            
            DialogEvents.RaiseValidSkillsChanged(newValidSkills);
        }
        public void UndoSkillModification(CardInstance source) => _skills.UndoSkillModification(source);

        /// <summary>
        /// Convenience function to determine if a card with the given skills can be played on the current set of valid
        /// skills for the check.
        /// </summary>
        /// <param name="skills">card skills</param>
        /// <returns>true if the current valid skills contains one or more of the given skills</returns>
        public bool CanUseSkill(params PF.Skill[] skills) => _skills.CanUseSkill(skills);

        public List<PF.Skill> GetCurrentValidSkills() => _skills.GetCurrentValidSkills();

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
            _skills = new CheckSkillAccumulator(Resolvable);
            _typeDeterminator = new CheckTypeDeterminator(Resolvable);
            _traits = new TraitAccumulator(Resolvable);
            ContextData.Clear();
        }
    }
}
