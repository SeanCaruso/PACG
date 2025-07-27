using System.Collections.Generic;
using UnityEngine;

public class CheckContext
{
    // Fields that must be set on initialization - these should never change.
    public CheckCategory CheckCategory { get; private set; }
    public LogicRegistry LogicRegistry { get; private set; }

    public CheckContext(CheckCategory checkCategory, LogicRegistry logicRegistry)
    {
        CheckCategory = checkCategory;
        LogicRegistry = logicRegistry;
    }

    // Fields that are populated as actions are processed.
    public PF.Skill UsedSkill { get; set; } = new();
    public List<PF.CardType> PlayedCardTypes { get; private set; } = new();
    public List<string> Traits { get; private set; } = new();
    public DicePool DicePool { get; private set; } = new();
    public int BlessingCount { get; set; } = 0;
    public Dictionary<string, object> ContextData { get; private set; } = new();
}
