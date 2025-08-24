using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public enum EncounterPhase
    {
        OnEncounter,
        Evasion,
        BeforeActing,
        AttemptCheck, // Might happen multiple times
        AfterActing,
        Resolve,
        Avenge
    }

    public class EncounterContext
    {
        // Basic information - set on construction
        public PlayerCharacter Character { get; }
        public CardInstance Card { get; }

        // Convenience properties
        public CardData CardData => Card.Data;

        /// <summary>
        /// Convenience function to check if the encountered card has any of the given traits.
        /// </summary>
        /// <param name="traits"></param>
        /// <returns>true if the encountered card has at least one of the given traits</returns>
        public bool HasTrait(params string[] traits) => traits.Any(trait => CardData.traits.Contains(trait));

        // Maps CardData to the list of traits that card prohibits.
        public Dictionary<PlayerCharacter, HashSet<string>> ProhibitedTraits { get; } = new();

        public EncounterPhase CurrentPhase { get; set; } = EncounterPhase.OnEncounter;
        public List<IExploreEffect> ExploreEffects { get; set; } = new();
        public CheckResult CheckResult { get; set; }

        public EncounterContext(PlayerCharacter pc, CardInstance card)
        {
            Character = pc;
            Card = card;
        }

        public void AddProhibitedTraits(PlayerCharacter pc, params string[] traits)
        {
            if (!ProhibitedTraits.ContainsKey(pc))
                ProhibitedTraits.Add(pc, new HashSet<string>());
            
            foreach (var trait in traits)
                ProhibitedTraits[pc].Add(trait);
        }

    }
}
