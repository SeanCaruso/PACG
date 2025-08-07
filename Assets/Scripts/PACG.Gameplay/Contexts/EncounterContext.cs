using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class EncounterContext
    {
        // Basic information - set on construction
        public PlayerCharacter EncounterPC { get; }
        public CardInstance EncounteredCard { get; }

        // Maps CardData to the list of traits that card prohibits.
        private readonly Dictionary<(PlayerCharacter, CardInstance), List<string>> prohibitedTraits = new();
        public Dictionary<(PlayerCharacter, CardInstance), List<string>> ProhibitedTraits => prohibitedTraits;

        public CheckResult CheckResult { get; set; }

        public EncounterContext(PlayerCharacter pc, CardInstance card)
        {
            EncounterPC = pc;
            EncounteredCard = card;
        }

        public void AddProhibitedTraits(PlayerCharacter pc, CardInstance card, params string[] traits)
        {
            if (!prohibitedTraits.ContainsKey((pc, card))) prohibitedTraits.Add((pc, card), new());
            foreach (var trait in traits) prohibitedTraits[(pc, card)].Add(trait);
        }
        public void UndoProhibitedTraits(PlayerCharacter pc, CardInstance card) => prohibitedTraits.Remove((pc, card));

    }
}