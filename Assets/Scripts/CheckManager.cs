
using System.Collections.Generic;
using UnityEngine;

public enum CheckMode { Single, Choice, Sequential }
public enum CheckCategory { Combat, Skill }

[System.Serializable]
public class CheckStep
{
    [Header("DC")]
    public int baseDC;
    public int adventureLevelMult;

    [Header("Check Type")]
    public CheckCategory category;
    public List<PF.Skill> allowedSkills;

    public int TotalDC(GameContext gameContext) => baseDC + (adventureLevelMult * gameContext.AdventureNumber);
}

[System.Serializable]
public class CheckRequirement
{
    public CheckMode mode = CheckMode.Single;
    public List<CheckStep> checkSteps = new();
}