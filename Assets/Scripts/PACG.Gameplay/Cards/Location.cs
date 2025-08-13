using PACG.Data;
using UnityEngine;

namespace PACG.Gameplay
{
    public class Location
    {
        public LocationData LocationData { get; }
        public LocationLogicBase LocationLogic { get; }

        private readonly Deck _deck;

        public Location(LocationData data, LocationLogicBase logic, Deck deck)
        {
            LocationData = data;
            LocationLogic = logic;
            _deck = deck;
        }
    }
}