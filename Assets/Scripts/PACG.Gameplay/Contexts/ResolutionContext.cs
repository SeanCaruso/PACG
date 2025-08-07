using PACG.SharedAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class ResolutionContext
    {
        public IResolvable CurrentResolvable { get; }
        private List<IStagedAction> _stagedActions = new();
        public IReadOnlyList<IStagedAction> StagedActions => _stagedActions;

        public bool IsResolved(List<IStagedAction> actions) => CurrentResolvable?.IsResolved(actions) ?? true;

        public ResolutionContext(IResolvable resolvable)
        {
            CurrentResolvable = resolvable;
        }

        public void StageAction(IStagedAction action)
        {

        }
    }
}
