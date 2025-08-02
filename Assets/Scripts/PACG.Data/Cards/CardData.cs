using PACG.Core.Characters;
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
    public int rerollThreshold = 0;

    [Header("Powers")]
    [TextArea(3,10)]
    public string powers;
    [TextArea(3,10)]
    public string recovery;
    public List<string> immunities;
    public List<string> vulnerabilities;

    [Header("Traits")]
    public List<string> traits;

    [Header("Set During Gameplay")]
    public PlayerCharacter OriginalOwner { get; set; } = null;
    public PlayerCharacter Owner { get; set; } = null;
}
