using System.Collections.Generic;
using UnityEngine;

public class EncounterContext
{
    public CardData EncounteredCardData { get; }
    public EncounterManager EncounterManager { get; }

    public EncounterContext(CardData card, EncounterManager encounterManager)
    {
        EncounteredCardData = card;
        EncounterManager = encounterManager;
    }

    // Set during the encounter
    public List<string> ProhibitedTraits { get; } = new();
    public CheckResult CheckResult { get; set; }
}
