using System;
using System.Collections.Generic;
using UnityEngine;

[PlayableLogicFor("Orison")]
public class OrisonLogic : BasePlayableLogic
{
    private const int DiscardToBless = 0;
    private const int RechargeToBless = 1;
    private const int DiscardToExplore = 2;

    public override List<PlayCardAction> GetAvailableActions(ActionContext context)
    {
        List<PlayCardAction> actions = new();
        actions.Add(new(this, CardData, PF.ActionType.Discard, powerIndex: DiscardToBless));

        if (context.TurnContext.HourBlessing.cardLevel == 0)
        {
            actions.Add(new(this, CardData, PF.ActionType.Recharge, powerIndex: RechargeToBless));
        }
        return actions;
    }

    public override void OnStage(ActionContext context, int? powerIndex = null)
    {
        //context.ResolutionManager.StageAction(new());
    }

    public override void ExecuteCardLogic(ActionContext context, int? powerIndex = null)
    {
        if (powerIndex == DiscardToBless || powerIndex == RechargeToBless)
        {
            ++context.BlessingCount;
        }
    }
}
