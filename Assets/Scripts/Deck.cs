using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<CardData> cards = new();
    private readonly List<CardData> activeDeck = new();

    void Awake()
    {
        activeDeck.AddRange(cards);
        Shuffle();
    }

    public void Shuffle()
    {
        for (int i = activeDeck.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            CardData temp = activeDeck[i];
            activeDeck[i] = activeDeck[randomIndex];
            activeDeck[randomIndex] = temp;
        }
    }

    public CardData DrawCard()
    {
        if (activeDeck.Count == 0)
        {
            return null;
        }

        CardData drawnCard = activeDeck[0];
        activeDeck.RemoveAt(0);
        return drawnCard;
    }

    public void Recharge(CardData card)
    {
        activeDeck.Add(card);
    }

    public void Reload(CardData card)
    {
        activeDeck.Insert(0, card);
    }
}
