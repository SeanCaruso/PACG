using System;
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
/*
[PlayableLogicFor("Orison")]
public class OrisonLogic : CardLogicBase
{
    public CardData CardData { get; set; }

    private const int DiscardToBless = 0;
    private const int RechargeToBless = 1;
    private const int DiscardToExplore = 2;

    protected override List<IStagedAction> GetAvailableCardActions()
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

    public override void OnStage(int? powerIndex = null)
    {
    }

    public override void OnUndo(int? powerIndex = null)
    {
    }

    public override void Execute(int? powerIndex = null)
    {
        if (powerIndex == DiscardToBless || powerIndex == RechargeToBless)
        {
            ++Contexts.CheckContext.BlessingCount;
        }
    }
}
*/
}