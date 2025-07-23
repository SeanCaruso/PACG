using System;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class PlayableLogicForAttribute : Attribute
{
    public string CardID { get; set; }
    public PlayableLogicForAttribute(string cardID) { this.CardID = cardID; }
}

public abstract class BasePlayableLogic
{
    public CardData CardData { get; set; }

    public bool CanPlay(ActionContext context) => GetAvailableActions(context).Count > 0;
    public abstract List<PlayCardAction> GetAvailableActions(ActionContext context);
    public abstract void OnStage(ActionContext context, int? powerIndex = null);

    public void Execute(ActionContext context, int? powerIndex = null)
    {
        if (!context.PlayedCardTypes.Contains(CardData.cardType))
        {
            context.PlayedCardTypes.Add(CardData.cardType);
        }

        ExecuteCardLogic(context, powerIndex);
    }

    public abstract void ExecuteCardLogic(ActionContext context, int? powerIndex = null);
}
