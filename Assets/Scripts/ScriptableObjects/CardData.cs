using System.Collections.Generic;
using UnityEngine;

public abstract class CardData : ScriptableObject
{
    [Header("Unique Identifier")]
    [Tooltip("The unique, programmatic ID for this card.")]
    public string cardID;

    [Header("Base Card Info")]
    public string cardName;
    public PF.CardType cardType;
    public int cardLevel;
    public Sprite cardArt;

    public CheckRequirement checkRequirement;

    public List<string> traits;
}
