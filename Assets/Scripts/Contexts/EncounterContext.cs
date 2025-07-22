using System.Collections.Generic;
using UnityEngine;

public class EncounterContext
{
    public CardData EncounteredCardData { get; }
    public PlayerCharacter ActivePlayer {  get; }
    public EncounterManager EncounterManager { get; }
    public CheckResult CheckResult { get; set; }

    public EncounterContext(CardData card, PlayerCharacter activePlayer, EncounterManager encounterManager)
    {
        EncounteredCardData = card;
        ActivePlayer = activePlayer;
        EncounterManager = encounterManager;
    }
}
