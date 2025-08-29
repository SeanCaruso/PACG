using System;
using System.Collections.Generic;
using PACG.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PACG.Gameplay
{
    public class Location : ICard, IExaminable
    {
        // ICard properties
        public string Name => LocationData.LocationName;
        public CardType CardType => CardType.Location;
        public List<string> Traits => LocationData.Traits;
        
        public LocationData LocationData { get; }
        private LocationLogicBase LocationLogic { get; }

        public Deck Deck { get; }
        public int Count => Deck.Count;
        public void Shuffle() => Deck.Shuffle();

        public override string ToString() => LocationData.LocationName;

        private readonly Dictionary<CardType, int> _knownComposition = new();
        private int _unknownCardCount;

        // Dependency injection
        private readonly ContextManager _contexts;

        public Location(LocationData data, LocationLogicBase logic, GameServices gameServices)
        {
            LocationData = Object.Instantiate(data);
            LocationLogic = logic;

            Deck = new Deck(gameServices.Cards);

            // We know how many of each card type are in the location initially.
            foreach (CardType type in Enum.GetValues(typeof(CardType)))
                _knownComposition[type] = 0;

            _contexts = gameServices.Contexts;
        }

        public CardInstance DrawCard()
        {
            var card =Deck.DrawCard();

            if (_knownComposition[card.Data.cardType] != 0)
                _knownComposition[card.Data.cardType]--;
            else if (_unknownCardCount > 0)
                _unknownCardCount--;
            else
                Debug.LogError($"[{LocationData.LocationName}] Drew a card we both did and didn't know the type of: {card.Data.cardType}.");

            return card;
        }

        public List<CardInstance> ExamineTop(int count) => Deck.ExamineTop(count);

        public void ShuffleIn(CardInstance card, bool isTypeKnown)
        {
            if (card == null) return;
            
            Deck.ShuffleIn(card);

            if (isTypeKnown)
                _knownComposition[card.Data.cardType]++;
            else
                _unknownCardCount++;
        }

        public IReadOnlyCollection<PlayerCharacter> Characters => _contexts.GameContext.GetCharactersAt(this);

        public void Close()
        {
            // Entangled and Frightened Scourges are removed on location close.
            foreach (var pc in Characters)
            {
                pc.RemoveScourge(ScourgeType.Entangled);
                pc.RemoveScourge(ScourgeType.Frightened);
            }
        }

        // ==============================================================================
        // CONVENIENCE FUNCTIONS
        // ==============================================================================
        
        // Facade pattern for LocationLogic
        public LocationPower? GetStartOfTurnPower() => LocationLogic?.GetStartOfTurnPower(this);
        public LocationPower? GetEndOfTurnPower() => LocationLogic?.GetEndOfTurnPower(this);
    }
}
