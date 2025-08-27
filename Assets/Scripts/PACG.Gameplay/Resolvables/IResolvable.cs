using System.Collections.Generic;
using PACG.SharedAPI;

namespace PACG.Gameplay
{
    public interface IResolvable
    {
        public void Initialize();
        public List<IStagedAction> GetAdditionalActionsForCard(CardInstance card);
        public bool CanCommit(List<IStagedAction> actions);
        IProcessor CreateProcessor(GameServices gameServices);
        public void Resolve();
        public StagedActionsState GetUIState(List<IStagedAction> actions);
        public void OnSkip();

        public bool CancelAbortsPhase { get; }
    }
}
