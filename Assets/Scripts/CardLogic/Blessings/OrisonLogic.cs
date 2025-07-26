using System;
using System.Collections.Generic;
using UnityEngine;

[PlayableLogicFor("Orison")]
public class OrisonLogic : IPlayableLogic
{
    public CardData CardData { get; set; }

    private const int DiscardToBless = 0;
    private const int RechargeToBless = 1;
    private const int DiscardToExplore = 2;

    public List<PlayCardAction> GetAvailableActions()
    {
        List<PlayCardAction> actions = new()
        {
            new(this, CardData, PF.ActionType.Discard, powerIndex: DiscardToBless)
        };

        if (Game.TurnContext.HourBlessing.cardLevel == 0)
        {
            actions.Add(new(this, CardData, PF.ActionType.Recharge, powerIndex: RechargeToBless));
        }
        return actions;
    }

    public void OnStage(int? powerIndex = null)
    {
        //context.ResolutionManager.StageAction(new());
    }

    public void ExecuteCardLogic(int? powerIndex = null)
    {
        if (powerIndex == DiscardToBless || powerIndex == RechargeToBless)
        {
            ++Game.ActionContext.BlessingCount;
        }
    }
}
