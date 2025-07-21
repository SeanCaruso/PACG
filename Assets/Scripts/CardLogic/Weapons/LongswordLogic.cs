using System;
using System.Collections.Generic;
using UnityEngine;

[PlayableLogicFor("Longsword")]
public class LongswordLogic : IPlayableLogic
{
    public List<PlayCardAction> GetAvailableActions(ActionContext context)
    {
        List<PlayCardAction> actions = new();
        if (context.CheckCategory == CheckCategory.Combat)
        {
            actions.Add(new(this, PF.ActionType.Reveal, powerIndex: 0));

            if (context.ActiveCharacter.IsProficient(PF.CardType.Weapon))
            {
                actions.Add(new(this, PF.ActionType.Reload, powerIndex: 1));
            }
        }
        return actions;
    }

    public void OnStage(ActionContext context, int? powerIndex = null)
    {
        //context.ResolutionManager.StageAction(new());
    }

    public void Execute()
    {
    }
}
