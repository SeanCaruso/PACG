using System.Collections.Generic;
using UnityEngine;

public class EncounterContext
{
    public PlayerCharacter EncounterPC { get; }
    public CardData EncounteredCardData { get; }

    public EncounterContext(PlayerCharacter pc, CardData card)
    {
        EncounterPC = pc;
        EncounteredCardData = card;
    }

    // Set during the encounter
    public Dictionary<(PlayerCharacter, CardData), List<string>> ProhibitedTraits { get; } = new(); // Maps CardData to the list of traits that card prohibits.
    public void AddProhibitedTraits(PlayerCharacter pc, CardData card, params string[] traits)
    {
        if (!ProhibitedTraits.ContainsKey((pc, card))) ProhibitedTraits.Add((pc, card), new());
        foreach (var trait in traits) ProhibitedTraits[(pc, card)].Add(trait);
    }

    public CheckResult CheckResult { get; set; }
}
