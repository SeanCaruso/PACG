using System.Collections.Generic;
using UnityEngine;

public class EncounterContext
{
    public TurnContext TurnContext { get; private set; }
    public CardData EncounteredCardData { get; }
    public EncounterManager EncounterManager { get; }
    public CheckResult CheckResult { get; set; }

    public EncounterContext(TurnContext turnContext, CardData card, EncounterManager encounterManager)
    {
        TurnContext = turnContext;
        EncounteredCardData = card;
        EncounterManager = encounterManager;
    }
}
