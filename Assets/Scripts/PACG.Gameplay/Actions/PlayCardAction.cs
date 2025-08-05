using System.Collections.Generic;

namespace PACG.Gameplay
{
    public class PlayCardAction : IStagedAction
    {
        // Data common to all staged actions.
        public IPlayableLogic Playable { get; private set; }
        public CardInstance Card { get; private set; }
        public PF.ActionType ActionType { get; private set; }

        // Dictionary to hold any custom data.
        public Dictionary<string, object> ActionData { get; } = new();

        private readonly string label = null;

        public PlayCardAction(IPlayableLogic playable, CardInstance card, PF.ActionType actionType, params (string, object)[] actionData)
        {
            this.Playable = playable;
            this.Card = card;
            this.ActionType = actionType;

            foreach ((string key, object value) in actionData)
                ActionData.Add(key, value);
        }

        // Convenience methods
        public bool IsCombat => (bool)ActionData.GetValueOrDefault("IsCombat", false);
        public bool IsFreely => (bool)ActionData.GetValueOrDefault("IsFreely", false);

        public string GetLabel()
        {
            return $"{(label is null ? ActionType.ToString() : label)} {Card.Data.cardName}";
        }

        public void OnStage(CheckContext checkContext = null)
        {
            checkContext?.StageAction(this);
            Playable?.OnStage(this);
        }

        public void OnUndo(CheckContext checkContext = null)
        {
            checkContext?.UndoAction(this);
            Playable?.OnUndo(this);
        }

        public void Commit(CheckContext checkContext = null)
        {
            checkContext?.AddTraits(Card.Data.traits.ToArray());
            Playable.Execute(this);
            // The card instance on the PlayerCharacter was moved during staging, so don't do it here.
        }
    }
}
