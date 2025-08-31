using System.Collections.Generic;
using PACG.Core;

namespace PACG.Gameplay
{
    public class PlayCardAction : IStagedAction
    {
        // Data common to all staged actions.
        public CardInstance Card { get; private set; }
        public ActionType ActionType { get; private set; }
        
        // Data specific to this action.
        public CheckModifier CheckModifier { get; }

        // Dictionary to hold any custom data.
        public Dictionary<string, object> ActionData { get; } = new();

        private readonly string _label = null;

        public PlayCardAction(CardInstance card, ActionType actionType, CheckModifier checkModifier,
            params (string, object)[] actionData)
        {
            Card = card;
            ActionType = actionType;
            CheckModifier = checkModifier;

            foreach (var (key, value) in actionData)
                ActionData.Add(key, value);
        }

        // Convenience methods
        public bool IsCombat => (bool)ActionData.GetValueOrDefault("IsCombat", false);
        public bool IsFreely => (bool)ActionData.GetValueOrDefault("IsFreely", false);

        public string GetLabel()
        {
            return $"{_label ?? ActionType.ToString()} {Card.Data.cardName}";
        }

        public void Commit()
        {
            Card.Logic?.OnCommit(this);
            // The card instance on the PlayerCharacter was moved during staging, so don't do it here.
        }
    }
}
