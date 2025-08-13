using PACG.Data;
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class Location
    {
        public LocationData LocationData { get; }
        public LocationLogicBase LocationLogic { get; }

        private readonly Deck _deck;
        public int Count => _deck.Count;

        private readonly ContextManager _contexts;

        public Location(LocationData data, LocationLogicBase logic, GameServices gameServices)
        {
            LocationData = data;
            LocationLogic = logic;

            _deck = new(gameServices.Cards);

            _contexts = gameServices.Contexts;
        }

        public CardInstance DrawCard() => _deck.DrawCard();
        public CardInstance Examine(int index = 0) => _deck.Examine(index);
        public void ShuffleIn(CardInstance card) => _deck.ShuffleIn(card);

        public IReadOnlyCollection<PlayerCharacter> Characters => _contexts.GameContext.GetCharactersAt(this);
    }
}