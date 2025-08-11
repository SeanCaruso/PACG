using System.Collections.Generic;

namespace PACG.Gameplay
{
    public interface IResolvable
    {
        public List<IStagedAction> GetAdditionalActionsForCard(CardInstance card);
        public bool IsResolved(List<IStagedAction> actions);
        IProcessor CreateProcessor(GameServices gameServices);
    }
}