using System.Collections.Generic;

namespace PACG.Gameplay
{
    public interface IResolvable
    {
        public bool IsResolved(List<IStagedAction> actions);
    }
}