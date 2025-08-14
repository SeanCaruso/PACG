using PACG.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class Location : IExaminable
    {
        public LocationData LocationData { get; }
        public LocationLogicBase LocationLogic { get; }

        private readonly Deck _deck;
        public Deck Deck => _deck;
        public int Count => _deck.Count;

        public string DisplayName => LocationData.LocationName;
        public override string ToString() => LocationData.LocationName;

        private readonly Dictionary<PF.CardType, int> _knownComposition = new();
        private int _unknownCardCount = 0;

        // Dependency injection
        private readonly ContextManager _contexts;

        public Location(LocationData data, LocationLogicBase logic, GameServices gameServices)
        {
            LocationData = data;
            LocationLogic = logic;

            _deck = new(gameServices.Cards);

            // We know how many of each card type are in the location initially.
            foreach (PF.CardType type in Enum.GetValues(typeof(PF.CardType)))
                _knownComposition[type] = 0;

            _contexts = gameServices.Contexts;
        }

        public CardInstance DrawCard()
        {
            var card =_deck.DrawCard();

            if (_knownComposition[card.Data.cardType] != 0)
                _knownComposition[card.Data.cardType]--;
            else if (_unknownCardCount > 0)
                _unknownCardCount--;
            else
                Debug.LogError($"[{LocationData.LocationName}] Drew a card we both did and didn't know the type of: {card.Data.cardType}.");

            return card;
        }

        public List<CardInstance> ExamineTop(int count) => _deck.ExamineTop(count);

        public void ShuffleIn(CardInstance card, bool isTypeKnown)
        { 
            _deck.ShuffleIn(card);

            if (isTypeKnown)
                _knownComposition[card.Data.cardType]++;
            else
                _unknownCardCount++;
        }

        public IReadOnlyCollection<PlayerCharacter> Characters => _contexts.GameContext.GetCharactersAt(this);
    }
}
