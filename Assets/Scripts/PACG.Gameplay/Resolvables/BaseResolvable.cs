using System.Collections.Generic;
using PACG.SharedAPI;

namespace PACG.Gameplay
{
    public abstract class BaseResolvable : IResolvable
    {
        private IProcessor _nextProcessor;

        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Manually set the next processor that should take over after resolving.
        /// </summary>
        /// <param name="nextProcessor"></param>
        public void OverrideNextProcessor(IProcessor nextProcessor) => _nextProcessor = nextProcessor;

        public virtual IProcessor CreateProcessor(GameServices gameServices) => _nextProcessor;

        public virtual List<IStagedAction> GetAdditionalActionsForCard(CardInstance card) => new();

        /// <summary>
        /// This is called by ActionStagingManager to determine whether to show the Commit/Skip button.
        /// </summary>
        /// <param name="actions">Staged actions</param>
        /// <returns>whether the resolvable can be committed with the provided staged actions</returns>
        public virtual bool CanCommit(List<IStagedAction> actions) => true;

        public virtual void Resolve()
        {
        }
        
        public virtual void OnSkip()
        {
        }

        /// <summary>
        /// Default action button state - Commit/Skip if valid, Cancel if actions are staged.
        /// </summary>
        /// <param name="actions">List of staged actions</param>
        public virtual StagedActionsState GetUIState(List<IStagedAction> actions)
        {
            var canCommit = actions.Count > 0 && CanCommit(actions);
            var canSkip = actions.Count == 0 && CanCommit(actions);

            return new StagedActionsState(
                canCommit: canCommit,
                canSkip: canSkip,
                canCancel: actions.Count > 0 || CancelAbortsPhase,
                isExploreEnabled: false // ASM takes care of this one.
            );
        }

        public virtual bool CancelAbortsPhase => false;
    }
}
