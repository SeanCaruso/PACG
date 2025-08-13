
namespace PACG.Gameplay
{
    public class DefaultAction : IStagedAction
    {
        public CardInstance Card { get; protected set; }
        private readonly PF.ActionType _actionType;
        public PF.ActionType ActionType => _actionType;
        public bool IsFreely => true;

        public DefaultAction(CardInstance card, PF.ActionType actionType)
        {
            Card = card;
            _actionType = actionType;
        }

        public void OnStage() { }

        public void OnUndo() { }

        public void Commit(CheckContext _ = null) { }
    }
}
