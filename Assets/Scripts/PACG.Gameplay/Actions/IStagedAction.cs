
namespace PACG.Gameplay
{
    public interface IStagedAction
    {
        public CardInstance Card { get; }
        public PF.ActionType ActionType { get; }
        public bool IsFreely { get; }

        public void OnStage();
        public void OnUndo();
        public void Commit(CheckContext checkContext = null); // CheckContext is required for traits
    }
}