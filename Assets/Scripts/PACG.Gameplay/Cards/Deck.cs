using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class Deck
    {
        private readonly List<CardInstance> _cards = new(); // Current deck state
        public int Count => _cards.Count;

        private readonly CardManager _cardManager;

        public Deck(CardManager cardManager)
        {
            _cardManager = cardManager;
        }

        public void Shuffle()
        {
            for (int i = _cards.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (_cards[randomIndex], _cards[i]) = (_cards[i], _cards[randomIndex]);
            }
        }

        public CardInstance DrawCard()
        {
            if (_cards.Count == 0) return null;

            CardInstance drawnCard = _cards[0];
            _cards.RemoveAt(0);
            return drawnCard;
        }

        public void Recharge(CardInstance card)
        {
            if (card == null || _cards.Contains(card)) return;
            _cards.Add(card);
            _cardManager.MoveCard(card, CardLocation.Deck);
        }

        public void Reload(CardInstance card)
        {
            if (card == null || _cards.Contains(card)) return;
            _cards.Insert(0, card);
            _cardManager.MoveCard(card, CardLocation.Deck);
        }

        public void ShuffleIn(CardInstance card)
        {
            Reload(card);
            Shuffle();
        }
    }
}
