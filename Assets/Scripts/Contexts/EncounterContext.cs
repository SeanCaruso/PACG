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
    public List<string> ProhibitedTraits { get; } = new();
    public CheckResult CheckResult { get; set; }
}
