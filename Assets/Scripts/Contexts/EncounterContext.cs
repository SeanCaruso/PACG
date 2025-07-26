using System.Collections.Generic;
using UnityEngine;

public class EncounterContext
{
    public CardData EncounteredCardData { get; }
    public EncounterManager EncounterManager { get; }
    public CheckResult CheckResult { get; set; }

    public EncounterContext(CardData card, EncounterManager encounterManager)
    {
        EncounteredCardData = card;
        EncounterManager = encounterManager;
    }
}
