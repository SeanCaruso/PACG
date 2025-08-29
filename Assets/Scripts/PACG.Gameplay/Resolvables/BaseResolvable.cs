using System.Collections.Generic;
using PACG.Data;
using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    public abstract class BaseResolvable : IResolvable
    {
        private readonly HashSet<CardType> _stagedCardTypes = new();
        
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
        public virtual bool CanCommit(IReadOnlyList<IStagedAction> actions) => true;

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
        public virtual StagedActionsState GetUIState(IReadOnlyList<IStagedAction> actions)
        {
            var canCommit = actions.Count > 0 && CanCommit(actions);
            var canSkip = actions.Count == 0 && CanCommit(actions);

            return new StagedActionsState
            {
                IsCommitButtonVisible = canCommit,
                IsSkipButtonVisible = canSkip,
                IsCancelButtonVisible = actions.Count > 0 || CancelAbortsPhase,
            };
        }

        public virtual bool CancelAbortsPhase => false;

        // =====================================================================================
        // RESOLVABLE-SPECIFIC ACTION STAGING
        //
        // Note: ActionStagingManager is responsible for the actual actions and cards that have
        //       been staged, but is rule-agnostic. BaseResolvable contains the rule-specific
        //       logic about which actions *can* be staged during a Resolvable.
        // =====================================================================================
        public IReadOnlyCollection<CardType> StagedCardTypes => _stagedCardTypes;
        public bool IsCardTypeStaged(CardType cardType) => _stagedCardTypes.Contains(cardType);

        public bool CanStageAction(IStagedAction action)
        {
            // Rule: prevent duplicate card types (if not freely playable).
            if (action.IsFreely || !_stagedCardTypes.Contains(action.Card.Data.cardType)) return true;
            
            Debug.LogWarning($"{action.Card.Data.cardName} staged a duplicate type - was this intended?");
            return false;

        }

        public void StageCardTypeIfNeeded(IStagedAction action)
        {
            if (!action.IsFreely) _stagedCardTypes.Add(action.Card.Data.cardType);
        }
    }
}
