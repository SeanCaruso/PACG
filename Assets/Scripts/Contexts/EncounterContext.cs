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
    private readonly Dictionary<(PlayerCharacter, CardData), List<string>> prohibitedTraits = new(); // Maps CardData to the list of traits that card prohibits.
    public Dictionary<(PlayerCharacter, CardData), List<string>> ProhibitedTraits => prohibitedTraits;
    public void AddProhibitedTraits(PlayerCharacter pc, CardData card, params string[] traits)
    {
        if (!prohibitedTraits.ContainsKey((pc, card))) prohibitedTraits.Add((pc, card), new());
        foreach (var trait in traits) prohibitedTraits[(pc, card)].Add(trait);
    }
    public void UndoProhibitedTraits(PlayerCharacter pc, CardData card) => prohibitedTraits.Remove((pc, card));

    public CheckResult CheckResult { get; set; }
}
