using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter
{
    private List<PF.CardType> proficiencies;

    public Deck deck;
    public List<CardData> hand;
    public List<CardData> discards;
    public List<CardData> buriedCards;
    public List<CardData> displayedCards;

    public bool IsProficient(PF.CardType cardType) => proficiencies.Contains(cardType);
}
