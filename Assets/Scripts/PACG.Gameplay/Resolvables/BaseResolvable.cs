using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public abstract class BaseResolvable : IResolvable
    {
        public virtual void Initialize() { }

        public virtual IProcessor CreateProcessor(GameServices gameServices) => null;

        public virtual List<IStagedAction> GetAdditionalActionsForCard(CardInstance card) => new();

        /// <summary>
        /// This is called by ActionStagingManager to determine whether or not to show the Commit/Skip button.
        /// </summary>
        /// <param name="actions">Staged actions</param>
        /// <returns>whether the resolvable can be committed with the provided staged actions</returns>
        public virtual bool CanCommit(List<IStagedAction> actions) => true;

        public virtual void Resolve() { }

        public virtual bool CancelAbortsPhase => false;
    }
}
