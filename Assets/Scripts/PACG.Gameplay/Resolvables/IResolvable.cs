using System.Collections.Generic;
using PACG.Data;
using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    public interface IResolvable
    {
        public void Initialize();
        public List<IStagedAction> GetAdditionalActionsForCard(CardInstance card);
        public bool CanCommit(IReadOnlyList<IStagedAction> actions);
        IProcessor CreateProcessor(GameServices gameServices);
        public void Resolve();
        public StagedActionsState GetUIState(IReadOnlyList<IStagedAction> actions);
        public void OnSkip();
        public bool CancelAbortsPhase { get; }
        
        // Card Staging
        public IReadOnlyCollection<CardType> StagedCardTypes { get; }
        public bool IsCardTypeStaged(CardType cardType);
        public bool CanStageAction(IStagedAction action);
        public void StageCardTypeIfNeeded(IStagedAction action);
    }
}
