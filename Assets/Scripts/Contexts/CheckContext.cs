using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CheckPhase
{
    PlayCards,
    RollDice,
    SufferDamage
}

public class CheckContext
{
    // Fields that must be set on initialization - these should never change.
    public PlayerCharacter CheckPC { get; }
    public CheckCategory CheckCategory { get; private set; }
    public List<PF.Skill> BaseValidSkills { get; private set; }

    public CheckContext(PlayerCharacter pc, CheckCategory checkCategory, List<PF.Skill> validSkills)
    {
        CheckPC = pc;
        CheckCategory = checkCategory;
        BaseValidSkills = validSkills;
    }

    // Fields that are populated as actions are processed.
    // Current check phase
    public CheckPhase CheckPhase { get; set; } = CheckPhase.PlayCards;

    // Currently staged info
    public List<PF.Skill> CurrentValidSkills => GetCurrentValidSkills();
    private readonly Dictionary<CardData, List<PF.Skill>> stagedSkillAdditions = new();
    private readonly Dictionary<CardData, List<PF.Skill>> stagedSkillRestrictions = new();

    public void AddValidSkills(CardData card, params PF.Skill[] skills)
    {
        stagedSkillAdditions.Add(card, new(skills));
    }
    public void RestrictValidSkills(CardData card, params PF.Skill[] skills)
    {
        stagedSkillRestrictions.Add(card, new(skills));
    }
    public void UndoSkillModification(CardData source)
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

    private List<PF.Skill> GetCurrentValidSkills()
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

    public List<CardData> StagedCards { get; private set; } = new();
    public List<PF.CardType> StagedCardTypes { get; private set; } = new();

    // Updated on action executions
    public PF.Skill UsedSkill { get; set; } = new();
    public List<string> Traits { get; private set; } = new();
    public DicePool DicePool { get; private set; } = new();
    public int BlessingCount { get; set; } = 0;
    public CheckResult CheckResult { get; set; } = null;

    // Custom data
    public Dictionary<string, object> ContextData { get; private set; } = new();
}