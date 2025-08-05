using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class Deck
    {
        private List<CardInstance> Cards { get; set; } = new(); // Current deck state
        public int Count => Cards.Count;

        public void Shuffle()
        {
            for (int i = Cards.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (Cards[randomIndex], Cards[i]) = (Cards[i], Cards[randomIndex]);
            }
        }

        public CardInstance DrawCard()
        {
            if (Cards.Count == 0) return null;

            CardInstance drawnCard = Cards[0];
            Cards.RemoveAt(0);
            return drawnCard;
        }

        public void Recharge(CardInstance card)
        {
            if (card == null || Cards.Contains(card)) return;
            Cards.Add(card);
        }

        public void Reload(CardInstance card)
        {
            if (card == null || Cards.Contains(card)) return;
            Cards.Insert(0, card);
        }

        public void ShuffleIn(CardInstance card)
        {
            Reload(card);
            Shuffle();
        }
    }
}
