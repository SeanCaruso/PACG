
using System.Collections.Generic;

namespace PACG.Gameplay
{
    /// <summary>
    /// A default card action that can be done freely (e.g. damage discards, optional discards, character powers, etc.)
    /// </summary>
    public class DefaultAction : IStagedAction
    {
        public CardInstance Card { get; protected set; }
        public PF.ActionType ActionType { get; }
        public bool IsFreely => true;
        public Dictionary<string, object> ActionData { get; } = new();

        public DefaultAction(CardInstance card, PF.ActionType actionType)
        {
            Card = card;
            ActionType = actionType;
        }

        public void Commit() { }
    }
}
