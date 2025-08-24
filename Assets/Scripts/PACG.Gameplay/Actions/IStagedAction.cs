using System.Collections.Generic;

namespace PACG.Gameplay
{
    public interface IStagedAction
    {
        public CardInstance Card { get; }
        public PF.ActionType ActionType { get; }
        public bool IsFreely { get; }
        // Dictionary to hold any custom data.
        public Dictionary<string, object> ActionData { get; }
        
        public void Commit();
    }
}
