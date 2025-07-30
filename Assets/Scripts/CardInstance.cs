using System;
using UnityEngine;

public class CardInstance
{
    public CardData Data { get; private set; }
    public Guid InstanceId { get; private set; }
    public CardLocation CurrentLocation { get; set; }
    public PlayerCharacter Owner { get; set; }
    public PlayerCharacter OriginalOwner { get; set; }

    public CardInstance(CardData data, PlayerCharacter owner = null)
    {
        Data = data;
        InstanceId = Guid.NewGuid();
        CurrentLocation = CardLocation.Deck;
        Owner = owner;
        OriginalOwner = owner;
    }
}

public enum CardLocation { Deck, Hand, Discard, Buried, Displayed }