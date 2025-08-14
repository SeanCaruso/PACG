using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PACG.Gameplay
{
    public class Deck
    {
        private readonly List<CardInstance> _cards = new(); // Current deck state
        public int Count => _cards.Count;

        private readonly HashSet<CardInstance> _examinedCards = new(); // Cards whose position is known.
        private readonly HashSet<CardInstance> _knownCards = new(); // Cards whose existence (but not position) is known.
        public bool KnowCardExists(CardInstance card) => _examinedCards.Contains(card) || _knownCards.Contains(card);

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
            _knownCards.UnionWith(_examinedCards);
            _examinedCards.Clear();
        }

        public CardInstance DrawCard()
        {
            if (_cards.Count == 0) return null;

            CardInstance drawnCard = _cards[0];
            _cards.RemoveAt(0);
            _examinedCards.Remove(drawnCard);
            _knownCards.Remove(drawnCard);
            return drawnCard;
        }

        public List<CardInstance> ExamineTop(int count)
        {
            var cards = new List<CardInstance>();
            for (int i = 0; i < count && i < _cards.Count; i++)
            {
                var card = _cards[i];
                cards.Add(card);
                _examinedCards.Add(card);
            }
            return cards;
        }

        /// <summary>
        /// This function dedicated to Bernard Sumner, Peter Hook, and Stephen Morris.
        /// </summary>
        /// <param name="newOrder">How... does it feel?</param>
        public void ReorderExamined(List<CardInstance> newOrder)
        {
            for (int i = 0; i < newOrder.Count; i++)
            {
                _cards[i] = newOrder[i];
            }
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

        public CardInstance DrawFirstCardWith(PF.CardType type, params string[] traits)
        {
            var traitList = traits.ToList();
            var matchingCards = _cards.Where(card => card.Data.cardType == type && (traitList.Count == 0 || traitList.Intersect(card.Data.traits).Any())).ToList();

            if (matchingCards.Count == 0)
                return null;
            else
            {
                var card = matchingCards[0];
                _cards.Remove(card);
                _examinedCards.Remove(card);
                return card;
            }
        }
    }
}
