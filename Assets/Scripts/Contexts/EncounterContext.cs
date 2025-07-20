using System.Collections.Generic;
using UnityEngine;

public class EncounterContext
{
    public GameContext GameContext { get; }

    public CardData EncounteredCardData { get; }
    public PlayerCharacter ActivePlayer {  get; }
    public EncounterManager EncounterManager { get; }
    public CheckResult CheckResult { get; set; }

    public EncounterContext(GameContext gameContext, CardData card, PlayerCharacter activePlayer, EncounterManager encounterManager)
    {
        GameContext = gameContext;
        EncounteredCardData = card;
        ActivePlayer = activePlayer;
        EncounterManager = encounterManager;
    }

    public IEncounterLogic GetEncounterLogic()
    {
        return GameContext.LogicRegistry.GetEncounterLogic(EncounteredCardData.cardID);
    }
}
