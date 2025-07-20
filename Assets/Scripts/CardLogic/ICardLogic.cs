using System;
using UnityEngine;

// --- The Encounter Interface ---
// For logic that triggers when this card IS the encounter.
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class EncounterLogicForAttribute : Attribute
{
    public string CardID {  get; set; }
    public EncounterLogicForAttribute(string cardID) {  this.CardID = cardID; }
}

public interface IEncounterLogic
{
    void Execute(EncounterContext context, EncounterPhase phase);
}

// --- The Player Action Hat Interface ---
// For logic that triggers on playable cards.
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class PlayableLogicForAttribute : Attribute
{
    public string CardID { get; set; }
    public PlayableLogicForAttribute(string cardID) { this.CardID = cardID; }
}

public interface IPlayableLogic
{
    bool CanPlay(GameContext context, EncounterPhase phase);
    void ExecuteOnPlay(GameContext context, EncounterPhase phase);
}
