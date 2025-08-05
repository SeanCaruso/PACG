
namespace PACG.Gameplay
{
    public class DefaultDamageAction : IStagedAction
    {
        public CardInstance Card { get; protected set; }
        public PF.ActionType ActionType => PF.ActionType.Discard;
        public bool IsFreely => true;
        public int Amount { get; protected set; }

        public DefaultDamageAction(CardInstance card)
        {
            Card = card;
        }

        public void OnStage(CheckContext _ = null) { }

        public void OnUndo(CheckContext _ = null) { }

        public void Commit(CheckContext _ = null)
        {
            Card.Owner.Discard(Card);
        }
    }
}
