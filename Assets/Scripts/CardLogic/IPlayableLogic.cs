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
    bool CanPlay(ActionContext context) => GetAvailableActions(context).Count > 0;
    List<PlayCardAction> GetAvailableActions(ActionContext context);
    void OnStage(ActionContext context, int? powerIndex = null);
    void Execute();
}
