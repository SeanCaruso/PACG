using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [Header("Design Time Test Setup")]
    public List<CardData> cardDatas = new();
    public PlayerCharacter character = null;

    public List<CardInstance> cards = new();
    private readonly List<CardInstance> activeDeck = new();

    void Awake()
    {
        cards.Clear();
        foreach (var cardData in cardDatas)
        {
            cards.Add(new(cardData, character));
        }

        activeDeck.AddRange(cards);
        Shuffle();
    }

    public void Shuffle()
    {
        for (int i = activeDeck.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (activeDeck[randomIndex], activeDeck[i]) = (activeDeck[i], activeDeck[randomIndex]);
        }
    }

    public CardInstance DrawCard()
    {
        if (activeDeck.Count == 0)
        {
            return null;
        }

        CardInstance drawnCard = activeDeck[0];
        activeDeck.RemoveAt(0);
        return drawnCard;
    }

    public void Recharge(CardInstance card)
    {
        activeDeck.Add(card);
    }

    public void Reload(CardInstance card)
    {
        activeDeck.Insert(0, card);
    }
}
