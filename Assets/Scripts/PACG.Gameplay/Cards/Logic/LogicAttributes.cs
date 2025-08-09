using System;

namespace PACG.Gameplay
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class LogicForAttribute : Attribute
    {
        public string CardID { get; set; }
        public LogicForAttribute(string cardID) { this.CardID = cardID; }
    }

    // Legacy attributes for backward compatibility during transition
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class EncounterLogicForAttribute : Attribute
    {
        public string CardID { get; set; }
        public EncounterLogicForAttribute(string cardID) { this.CardID = cardID; }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class PlayableLogicForAttribute : Attribute
    {
        public string CardID { get; set; }
        public PlayableLogicForAttribute(string cardID) { this.CardID = cardID; }
    }
}