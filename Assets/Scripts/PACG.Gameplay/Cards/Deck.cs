using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class Deck
    {
        private readonly List<CardInstance> _cards = new(); // Current deck state
        private readonly List<CardInstance> _examinedCards = new();

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
            _examinedCards.Clear();
        }

        public CardInstance DrawCard()
        {
            if (_cards.Count == 0) return null;

            CardInstance drawnCard = _cards[0];
            _cards.RemoveAt(0);
            _examinedCards.Remove(drawnCard);
            return drawnCard;
        }

        public CardInstance Examine(int idx)
        {
            if (idx >= _cards.Count) return null;

            var card = _cards[idx];
            if (!_examinedCards.Contains(card)) _examinedCards.Add(card);
            return card;
        }

        public void Recharge(CardInstance card)
        {
            if (card == null || _cards.Contains(card)) return;
            _cards.Add(card);
            _cardManager.MoveCard(card, CardLocation.Deck);
            _examinedCards.Add(card);
        }

        public void Reload(CardInstance card)
        {
            if (card == null || _cards.Contains(card)) return;
            _cards.Insert(0, card);
            _cardManager.MoveCard(card, CardLocation.Deck);
            _examinedCards.Add(card);
        }

        public void ShuffleIn(CardInstance card)
        {
            Reload(card);
            Shuffle();
        }
    }
}
