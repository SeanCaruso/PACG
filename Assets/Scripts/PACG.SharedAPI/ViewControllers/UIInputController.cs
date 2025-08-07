using PACG.Gameplay;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace PACG.SharedAPI
{
    public class UIInputController : MonoBehaviour
    {
        [Header("Turn Flow")]
        public Button giveCardButton;
        public Button moveButton;
        public Button exploreButton; // The location deck.
        public Button optionalDiscardButton;
        public Button endTurnButton;

        [Header("Action Staging Flow")]
        public Button cancelButton;
        public Button commitButton;
        public Button skipButton;

        // Dependency injections set in Initialize
        private TurnContext _turnContext;
        private GameFlowManager _gameFlowManager;
        private GameServices _gameServices;

        protected void Start()
        {
            // Subscribe to events.
            GameEvents.TurnStateChanged += UpdateTurnButtons;
            GameEvents.StagedActionsStateChanged += UpdateStagedActionButtons;

            giveCardButton.onClick.AddListener(() => GiveCardButton_OnClick());
            moveButton.onClick.AddListener(() => MoveButton_OnClick());
            exploreButton.onClick.AddListener(() => ExploreButton_OnClick());
            optionalDiscardButton.onClick.AddListener(() =>  OptionalDiscardButton_OnClick());
            endTurnButton.onClick.AddListener(() => EndTurnButton_OnClick());
        }

        protected void OnDestroy()
        {
            GameEvents.TurnStateChanged -= UpdateTurnButtons;
            GameEvents.StagedActionsStateChanged -= UpdateStagedActionButtons;
        }

        public void Initialize(GameServices gameServices)
        {
            _gameFlowManager = gameServices.GameFlow;
            _gameServices = gameServices;
            _turnContext = gameServices.Contexts.TurnContext;
        }

        // --- Turn Flow -----------------------------------------

        public void GiveCardButton_OnClick() { } //=> _gameFlowManager.QueueResolvable(new GiveCardResolvable());
        public void MoveButton_OnClick() { } //=> _turnProcessor.MoveToLocation();
        public void ExploreButton_OnClick() => _gameFlowManager.QueueAndProcess(new ExploreProcessor(_gameServices));
        public void OptionalDiscardButton_OnClick() { } //=> _turnProcessor.OptionalDiscards();
        public void EndTurnButton_OnClick() { } //=> _turnProcessor.EndTurn();

        protected void UpdateTurnButtons(TurnContext context)
        {
            giveCardButton.enabled = context.CanGive;
            moveButton.enabled = context.CanMove;
            exploreButton.enabled = context.CanExplore;
            optionalDiscardButton.enabled = context.CurrentPC.Hand.Count > 0;
        }

        // --- Action Staging Flow -----------------------------------

        public event Action OnCancelButtonClicked;
        public void CancelButton_OnClick() => OnCancelButtonClicked?.Invoke();

        public event Action OnCommitButtonClicked;
        public void CommitButton_OnClick() => OnCommitButtonClicked?.Invoke();

        public event Action OnSkipButtonClicked;
        public void SkipButton_OnClick() => OnSkipButtonClicked?.Invoke();

        protected void UpdateStagedActionButtons(StagedActionsState state)
        {
            cancelButton.gameObject.SetActive(state.IsCancelButtonVisible);
            commitButton.gameObject.SetActive(state.IsCommitButtonVisible);
            skipButton.gameObject.SetActive(state.IsSkipButtonVisible);
        }
    }
}
 
