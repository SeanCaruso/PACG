using System.Collections.Generic;
using PACG.Core;

namespace PACG.Gameplay
{
    public class PlayCardAction : IStagedAction
    {
        // Data common to all staged actions.
        public CardInstance Card { get; private set; }
        public PF.ActionType ActionType { get; private set; }

        // Dictionary to hold any custom data.
        public Dictionary<string, object> ActionData { get; } = new();

        private readonly string _label = null;

        public PlayCardAction(CardInstance card, PF.ActionType actionType, params (string, object)[] actionData)
        {
            Card = card;
            ActionType = actionType;

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

        public void OnStage() => Card.Logic?.OnStage(Card, this);

        public void OnUndo() => Card.Logic?.OnUndo(Card, this);

        public void Commit(CheckContext checkContext, DicePool dicePool)
        {
            Card.Logic?.Execute(Card, this, dicePool);
            // The card instance on the PlayerCharacter was moved during staging, so don't do it here.
        }
    }
}
