using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<CardData> cards = new List<CardData>();
    private List<CardData> _activeDeck = new List<CardData>();

    void Awake()
    {
        _activeDeck.AddRange(cards);
        Shuffle();
    }

    public void Shuffle()
    {
        for (int i = _activeDeck.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            CardData temp = _activeDeck[i];
            _activeDeck[i] = _activeDeck[randomIndex];
            _activeDeck[randomIndex] = temp;
        }
        Debug.Log("Deck shuffled.");
    }

    public CardData DrawCard()
    {
        if (_activeDeck.Count == 0)
        {
            Debug.LogWarning("Deck is empty!");
            return null;
        }

        CardData drawnCard = _activeDeck[0];
        _activeDeck.RemoveAt(0);
        return drawnCard;
    }
}
