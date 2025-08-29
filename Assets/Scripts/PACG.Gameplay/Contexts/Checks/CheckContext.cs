using System;
using System.Collections.Generic;
using System.Linq;
using PACG.Core;
using PACG.SharedAPI;

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
            _skills = new CheckSkillAccumulator(Resolvable);
            _typeDeterminator = new CheckTypeDeterminator(Resolvable);
            _traits = new TraitAccumulator(Resolvable);
            
            // Initialize UsedSkill to the PC's best valid skill.
            UsedSkill = Character.GetBestSkill(GetCurrentValidSkills().ToArray()).skill;
        }

        public void UpdatePreviewState(IReadOnlyList<IStagedAction> stagedActions)
        {
            // Start from a fresh state.
            _skills = new CheckSkillAccumulator(Resolvable);
            _typeDeterminator = new CheckTypeDeterminator(Resolvable);
            _traits = new TraitAccumulator(Resolvable);

            // Gather and apply modifiers.
            foreach (var action in stagedActions)
            {
                var modifier = action.Card.Logic?.GetCheckModifier(action);
                if (modifier != null)
                {
                    // Do this first - the skill selection dialog uses it to determine the DC for the selected
                    // skill, but if something forces a Combat check, we need to know that first.
                    if (modifier.RestrictedCategory != null)
                        _typeDeterminator.RestrictCheckCategory(modifier.SourceCard, modifier.RestrictedCategory.Value);
                    
                    _skills.RestrictValidSkills(modifier.SourceCard, modifier.RestrictedSkills.ToArray());
                    _skills.AddValidSkills(modifier.SourceCard, modifier.AddedValidSkills.ToArray());
                    _traits.AddTraits(modifier.SourceCard, modifier.AddedTraits.ToArray());
                    _traits.AddRequiredTraits(modifier.SourceCard, modifier.RequiredTraits.ToArray());
                    _traits.AddProhibitedTraits(modifier.SourceCard, modifier.ProhibitedTraits.ToArray());
                }
            }
            
            GameEvents.RaiseDicePoolChanged(DicePoolBuilder.Build(this, stagedActions));
            
            // Update the context.
            var newValidSkills = GetCurrentValidSkills();
            if (!newValidSkills.Contains(UsedSkill))
                UsedSkill = Character.GetBestSkill(newValidSkills.ToArray()).skill;
            
            DialogEvents.RaiseValidSkillsChanged(newValidSkills);
        }

        public List<Skill> GetCurrentValidSkills()
        {
            var validSkills = _skills.GetCurrentValidSkills();
            if (_traits.RequiredTraits.Count == 0) return validSkills;

            for (var i = validSkills.Count - 1; i >= 0; i--)
            {
                var skill = validSkills[i];
                var attr = Character.GetAttributeForSkill(skill);
                if (!_traits.RequiredTraits.Intersect(new[] { skill.ToString(), attr.ToString()}).Any())
                    validSkills.RemoveAt(i);
            }
            return validSkills;
        }

        // =====================================================================================
        // TRAITS
        // =====================================================================================
        // public void AddTraits(ICard card, params string[] traits) =>
        //     _traits?.Add(card, traits.Length > 0 ? traits : card.Traits.ToArray());
        // public void RemoveTraits(ICard card) => _traits.Remove(card);
        
        /// <summary>
        /// All traits currently invoked by the check.
        /// </summary>
        public IReadOnlyList<string> Traits
        {
            get
            {
                var traits = _traits.Traits.ToList();
                traits.Add(UsedSkill.ToString());
                if (Character.GetAttributeForSkill(UsedSkill) != UsedSkill)
                    traits.Add(Character.GetAttributeForSkill(UsedSkill).ToString());
                return traits;
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
        
        public IReadOnlyList<string> ProhibitedTraits(PlayerCharacter pc) => _traits.ProhibitedTraits(pc);
        
        // =====================================================================================
        // TYPE DETERMINATION PASSTHROUGHS TO CheckTypeDeterminator
        // =====================================================================================
        public bool IsCombatValid => _typeDeterminator.IsCombatValid;
        public bool IsSkillValid => _typeDeterminator.IsSkillValid;

        public int GetDcForSkill(Skill skill) => _typeDeterminator.GetDcForSkill(skill);

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

        /// <summary>
        /// Convenience function to determine if a card with the given skills can be played on the current set of valid
        /// skills for the check.
        /// </summary>
        /// <param name="skills">card skills</param>
        /// <returns>true if the current valid skills contains one or more of the given skills</returns>
        public bool CanUseSkill(params Skill[] skills) => _skills.CanUseSkill(skills);

        // =====================================================================================
        // CHECK RESULTS ENCAPSULATION
        // =====================================================================================
        public List<IStagedAction> CommittedActions { get; set; }
        public Skill UsedSkill { get; set; }
        public CheckResult CheckResult { get; set; }
        
        public DicePool DicePool(IReadOnlyList<IStagedAction> actions) => DicePoolBuilder.Build(this, actions);

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
