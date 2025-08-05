using System;
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class EncounterLogicForAttribute : Attribute
    {
        public string CardID {  get; set; }
        public EncounterLogicForAttribute(string cardID) {  this.CardID = cardID; }
    }

    public interface IEncounterLogic : ICardLogic
    {
        List<IResolvable> Execute(EncounterPhase phase);
    }
}
