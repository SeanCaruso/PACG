using System.Collections.Generic;

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

        // Maps CardData to the list of traits that card prohibits.
        public Dictionary<(PlayerCharacter, CardInstance), List<string>> ProhibitedTraits { get; } = new();

        public List<IExploreEffect> ExploreEffects { get; set; } = new();
        public CheckResult CheckResult { get; set; }

        public EncounterContext(PlayerCharacter pc, CardInstance card)
        {
            Character = pc;
            Card = card;
        }

        public void AddProhibitedTraits(PlayerCharacter pc, CardInstance card, params string[] traits)
        {
            if (!ProhibitedTraits.ContainsKey((pc, card))) ProhibitedTraits.Add((pc, card), new List<string>());
            foreach (var trait in traits) ProhibitedTraits[(pc, card)].Add(trait);
        }
        public void UndoProhibitedTraits(PlayerCharacter pc, CardInstance card) => ProhibitedTraits.Remove((pc, card));

    }
}
