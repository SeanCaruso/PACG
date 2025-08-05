
using System.Collections.Generic;

namespace PACG.Gameplay
{
    public class EncounterContext
    {
        public PlayerCharacter EncounterPC { get; }
        public CardInstance EncounteredCard { get; }

        public EncounterContext(PlayerCharacter pc, CardInstance card)
        {
            EncounterPC = pc;
            EncounteredCard = card;
        }

        // Set during the encounter
        private readonly Dictionary<(PlayerCharacter, CardInstance), List<string>> prohibitedTraits = new(); // Maps CardData to the list of traits that card prohibits.
        public Dictionary<(PlayerCharacter, CardInstance), List<string>> ProhibitedTraits => prohibitedTraits;
        public void AddProhibitedTraits(PlayerCharacter pc, CardInstance card, params string[] traits)
        {
            if (!prohibitedTraits.ContainsKey((pc, card))) prohibitedTraits.Add((pc, card), new());
            foreach (var trait in traits) prohibitedTraits[(pc, card)].Add(trait);
        }
        public void UndoProhibitedTraits(PlayerCharacter pc, CardInstance card) => prohibitedTraits.Remove((pc, card));

        public CheckResult CheckResult { get; set; }
    }
}