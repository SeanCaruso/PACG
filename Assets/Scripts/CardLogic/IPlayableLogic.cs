using System;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class PlayableLogicForAttribute : Attribute
{
    public string CardID { get; set; }
    public PlayableLogicForAttribute(string cardID) { this.CardID = cardID; }
}

public interface IPlayableLogic : ICardLogic
{
    public bool CanPlay => GetAvailableActions().Count > 0;
    public List<IStagedAction> GetAvailableActions();
    public void OnStage(int? powerIndex = null);

    public void Execute(int? powerIndex = null)
    {
        if (!Game.CheckContext.PlayedCardTypes.Contains(CardData.cardType))
        {
            Game.CheckContext.PlayedCardTypes.Add(CardData.cardType);
        }

        ExecuteCardLogic(powerIndex);
    }

    public void ExecuteCardLogic(int? powerIndex = null);
}
