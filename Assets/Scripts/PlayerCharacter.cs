using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter
{
    public List<PF.CardType> proficiencies = new();

    public Deck deck;
    public List<CardData> hand = new();
    public List<CardData> discards = new();
    public List<CardData> buriedCards = new();
    public List<CardData> displayedCards = new();

    public bool IsProficient(PF.CardType cardType) => proficiencies.Contains(cardType);
}
