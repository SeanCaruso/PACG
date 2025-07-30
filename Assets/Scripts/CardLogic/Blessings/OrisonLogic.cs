using System;
using System.Collections.Generic;
using UnityEngine;

/*
[PlayableLogicFor("Orison")]
public class OrisonLogic : IPlayableLogic
{
    public CardData CardData { get; set; }

    private const int DiscardToBless = 0;
    private const int RechargeToBless = 1;
    private const int DiscardToExplore = 2;

    public List<IStagedAction> GetAvailableCardActions()
    {
        // TODO: Update this entire class to match standards.
        List<IStagedAction> actions = new()
        {
            new PlayCardAction(this, CardData, PF.ActionType.Discard, powerIndex: DiscardToBless)
        };

        if (Game.TurnContext.HourBlessing.cardLevel == 0)
        {
            actions.Add(new PlayCardAction(this, CardData, PF.ActionType.Recharge, powerIndex: RechargeToBless));
        }
        return actions;
    }

    public void OnStage(int? powerIndex = null)
    {
    }

    public void OnUndo(int? powerIndex = null)
    {
    }

    public void Execute(int? powerIndex = null)
    {
        if (powerIndex == DiscardToBless || powerIndex == RechargeToBless)
        {
            ++Game.CheckContext.BlessingCount;
        }
    }
}
*/