using PACG.Core.Characters;
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
    // --- Immutable Initial State ---
    // These are set once and should never change.
    public PlayerCharacter CheckPC { get; }
    public CheckCategory CheckCategory { get; }
    public int TotalDC { get; }
    public IReadOnlyList<PF.Skill> BaseValidSkills { get; }

    public CheckContext(PlayerCharacter pc, CheckStep checkStep, GameContext gameContext)
    {
        CheckPC = pc;
        CheckCategory = checkStep.category;
        BaseValidSkills = checkStep.category == CheckCategory.Skill ? checkStep.allowedSkills : new() { PF.Skill.Strength, PF.Skill.Melee };

        TotalDC = checkStep.baseDC + (gameContext.AdventureNumber * checkStep.adventureLevelMult);
    }

    // --- Dynamic Check State ---
    public CheckPhase CheckPhase { get; set; } = CheckPhase.PlayCards;

    // --- Staged Actions & Cards (Encapsulated) ---
    // Backing fields are private. No one outside this class can touch the raw lists.
    private readonly List<IStagedAction> _stagedActions = new();
    private readonly HashSet<PF.CardType> _stagedCardTypes = new();

    // Public access is through a read-only interface. Prevents external .Add() or .Clear().
    public IReadOnlyList<IStagedAction> StagedActions => _stagedActions;
    public IReadOnlyCollection<PF.CardType> StagedCardTypes => _stagedCardTypes;
    public IEnumerable<CardInstance> StagedCards => _stagedActions.Select(action => action.Card).Distinct();

    // This is now the ONLY way to add an action to the check.
    public bool StageAction(IStagedAction action)
    {
        if (_stagedActions.Contains(action))
        {
            Debug.LogWarning($"{action.Card.Data.cardName}.{action} staged multiple times - was this intended?");
            return false;
        }

        _stagedActions.Add(action);
        if (!action.IsFreely)
        {
            if (!_stagedCardTypes.Add(action.Card.Data.cardType))
                Debug.LogWarning($"{action.Card.Data.cardName} staged a duplicate type - was this intended?");
        }
        return true;
    }

    public bool UndoAction(IStagedAction action)
    {
        if (_stagedActions.Remove(action))
        {
            if (!action.IsFreely)
            {
                if (!_stagedCardTypes.Remove(action.Card.Data.cardType))
                    Debug.LogError($"{action.Card.Data.cardName} attempted to undo its type without being staged!");
            }
            return true;
        }
        else
        {
            Debug.LogError($"{action.Card.Data.cardName} attempted to undo without being staged!");
            return false;
        }
    }

    // --- Skill Modifications ---
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
            stagedSkillRestrictions.Add(card,new(skills));
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

    // --- Check Results (Encapsulated) ---
    private readonly List<string> _traits = new();
    private readonly DicePool _dicePool = new();

    public PF.Skill UsedSkill { get; set; }
    public IReadOnlyList<string> Traits => _traits;
    public DicePool DicePool => _dicePool;
    public int BlessingCount { get; set; }
    public CheckResult CheckResult { get; set; }

    // Public methods to control state changes
    public void AddTraits(params string[] traits) => _traits.AddRange(traits);
    public void AddToDicePool(int count, int sides, int bonus = 0) => _dicePool.AddDice(count, sides, bonus); // Example

    // --- Custom Data ---
    public Dictionary<string, object> ContextData { get; } = new(); // Get-only to prevent replacement
}