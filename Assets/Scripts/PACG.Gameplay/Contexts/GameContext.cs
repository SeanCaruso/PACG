using System.Collections.Generic;
using System.Linq;
using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    public class GameContext
    {
        public int AdventureNumber { get; }
        public Deck HourDeck { get; }

        private readonly Dictionary<Location, List<PlayerCharacter>> _locationPcs = new();

        public IReadOnlyList<Location> Locations => _locationPcs.Keys.ToList();
        public IReadOnlyList<PlayerCharacter> Characters => _locationPcs.Values.SelectMany(x => x).ToList();
        public IReadOnlyList<PlayerCharacter> GetCharactersAt(Location loc) => _locationPcs.GetValueOrDefault(loc, new List<PlayerCharacter>());

        public GameContext(int adventureNumber, CardManager cardManager)
        {
            AdventureNumber = adventureNumber;
            HourDeck = new Deck(cardManager);
        }
        
        public void AddLocation(Location loc) => _locationPcs.TryAdd(loc, new List<PlayerCharacter>());

        public Location GetPcLocation(PlayerCharacter pc)
        {
            foreach (var (loc, list) in _locationPcs)
            {
                if (list.Contains(pc))
                    return loc;
            }

            Debug.LogWarning($"[{GetType().Name}] Unable to find location for {pc.CharacterData.CharacterName}!");
            return null;
        }

        public void MoveCharacter(PlayerCharacter pc, Location newLoc)
        {
            var oldLoc = GetPcLocation(pc);
            if (oldLoc == null) return;

            _locationPcs[oldLoc].Remove(pc);
            SetPcLocation(pc, newLoc);
            
            GameEvents.RaiseLocationChanged(newLoc);
        }

        /// <summary>
        /// THIS SHOULD ONLY BE CALLED MANUALLY ON GAME SETUP - CALL MoveCharacter INSTEAD
        /// </summary>
        /// <param name="pc"></param>
        /// <param name="newLoc"></param>
        public void SetPcLocation(PlayerCharacter pc, Location newLoc)
        {
            Debug.Log($"[{GetType().Name}] Moving {pc.CharacterData.CharacterName} to {newLoc.LocationData.LocationName}.");
            if (!_locationPcs.ContainsKey(newLoc))
                _locationPcs.Add(newLoc, new List<PlayerCharacter>());
            _locationPcs[newLoc].Add(pc);
        }
    }
}
