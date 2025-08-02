using PACG.Services.Game;
using UnityEngine;

[System.Serializable]
public abstract class CardPower
{
    public EncounterPhase encounterPhase;
    public string logicClassName;
    public string methodName;
}
