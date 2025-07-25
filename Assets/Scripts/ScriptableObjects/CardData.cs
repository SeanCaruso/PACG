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

    [Header("Check(s) to Acquire/Defeat")]
    public CheckRequirement checkRequirement;

    [Header("Powers")]
    public string powers;
    public string recovery;

    [Header("Traits")]
    public List<string> traits;
}
