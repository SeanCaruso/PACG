using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public abstract class BaseResolvable : IResolvable
    {
        public virtual IProcessor CreateProcessor(GameServices gameServices) => null;

        public virtual List<IStagedAction> GetAdditionalActionsForCard(CardInstance card) => new();

        public virtual bool IsResolved(List<IStagedAction> actions) => true;

        public virtual void Resolve() { }

        public virtual bool CancelAbortsPhase => false;
    }
}
