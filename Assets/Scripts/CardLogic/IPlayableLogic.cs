using System;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class PlayableLogicForAttribute : Attribute
{
    public string CardID { get; set; }
    public PlayableLogicForAttribute(string cardID) { this.CardID = cardID; }
}

public interface IPlayableLogic
{
    bool CanPlay(ActionContext context, CardData thisCardData) => GetAvailableActions(context, thisCardData).Count > 0;
    List<PlayCardAction> GetAvailableActions(ActionContext context, CardData thisCardData);
    void OnStage(ActionContext context, int? powerIndex = null);
    void Execute(ActionContext context, int? powerIndex = null);
}
