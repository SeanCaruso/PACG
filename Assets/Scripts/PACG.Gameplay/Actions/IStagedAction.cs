using System.Collections.Generic;
using PACG.Core;

namespace PACG.Gameplay
{
    public interface IStagedAction
    {
        public CardInstance Card { get; }
        public ActionType ActionType { get; }
        public bool IsFreely { get; }
        // Dictionary to hold any custom data.
        public Dictionary<string, object> ActionData { get; }
        
        public void Commit();
    }
}
