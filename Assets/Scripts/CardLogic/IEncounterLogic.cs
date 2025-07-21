using System;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class EncounterLogicForAttribute : Attribute
{
    public string CardID {  get; set; }
    public EncounterLogicForAttribute(string cardID) {  this.CardID = cardID; }
}

public interface IEncounterLogic
{
    List<IResolvable> Execute(PlayerCharacter character, EncounterPhase phase);
}
