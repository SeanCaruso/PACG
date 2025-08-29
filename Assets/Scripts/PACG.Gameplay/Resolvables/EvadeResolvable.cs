using System;
using System.Collections.Generic;
using PACG.SharedAPI;

namespace PACG.Gameplay
{
    public class EvadeResolvable : BaseResolvable
    {
        private readonly Action _onEvadeCallback;

        public EvadeResolvable(Action onEvadeCallback)
        {
            _onEvadeCallback = onEvadeCallback;
        }
        
        public override void Resolve() => _onEvadeCallback?.Invoke();
        
        public override StagedActionsState GetUIState(List<IStagedAction> actions)
        {
            var canCommit = actions.Count > 0;
            var canSkip = actions.Count == 0;

            return new StagedActionsState
            {
                IsCommitButtonVisible = canCommit,
                IsSkipButtonVisible = canSkip,
                IsCancelButtonVisible = actions.Count > 0 || CancelAbortsPhase,
            };
        }
    }
}
