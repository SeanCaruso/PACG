using PF;
using System.Collections.Generic;
using UnityEngine;

public class ActionContext
{
    public PlayerCharacter ActiveCharacter { get; private set; }
    public CheckCategory CheckCategory { get; private set; }
    public ResolutionManager ResolutionManager { get; private set; }

    public LogicRegistry LogicRegistry { get; private set; }

    public ActionContext(PlayerCharacter character, CheckCategory checkCategory, ResolutionManager resolutionManager, LogicRegistry logicRegistry)
    {
        ActiveCharacter = character;
        CheckCategory = checkCategory;
        ResolutionManager = resolutionManager;
        LogicRegistry = logicRegistry;
    }
}
