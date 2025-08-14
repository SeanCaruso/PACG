using System.Collections.Generic;

namespace PACG.Gameplay
{
    public interface IResolvable
    {
        public void Initialize();
        public List<IStagedAction> GetAdditionalActionsForCard(CardInstance card);
        public bool CanCommit(List<IStagedAction> actions);
        IProcessor CreateProcessor(GameServices gameServices);
        public void Resolve();

        public bool CancelAbortsPhase { get; }
    }
}
