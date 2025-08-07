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
        private ActionStagingManager _asm;
        private GameFlowManager _gameFlowManager;
        private GameServices _gameServices;

        protected void Start()
        {
            // Subscribe to events.
            GameEvents.TurnStateChanged += UpdateTurnButtons;
            GameEvents.StagedActionsStateChanged += UpdateStagedActionButtons;

            // Hook up button clicks.
            //giveCardButton.onClick.AddListener(() => GiveCardButton_OnClick());
            //moveButton.onClick.AddListener(() => MoveButton_OnClick());
            exploreButton.onClick.AddListener(() => _gameFlowManager.QueueAndProcess(new ExploreProcessor(_gameServices)));
            //optionalDiscardButton.onClick.AddListener(() =>  OptionalDiscardButton_OnClick());
            //endTurnButton.onClick.AddListener(() => EndTurnButton_OnClick());

            cancelButton.onClick.AddListener(() => _asm.Cancel());
            commitButton.onClick.AddListener(() => _asm.Commit());
            skipButton.onClick.AddListener(() => _asm.Commit());
        }

        protected void OnDestroy()
        {
            GameEvents.TurnStateChanged -= UpdateTurnButtons;
            GameEvents.StagedActionsStateChanged -= UpdateStagedActionButtons;
        }

        public void Initialize(GameServices gameServices)
        {
            _asm = gameServices.ASM;
            _gameFlowManager = gameServices.GameFlow;
            _gameServices = gameServices;
        }

        // --- Turn Flow -----------------------------------------

        protected void UpdateTurnButtons(TurnContext context)
        {
            giveCardButton.enabled = context.CanGive;
            moveButton.enabled = context.CanMove;
            exploreButton.enabled = context.CanExplore;
            optionalDiscardButton.enabled = context.CurrentPC.Hand.Count > 0;
        }

        // --- Action Staging Flow -----------------------------------

        protected void UpdateStagedActionButtons(StagedActionsState state)
        {
            cancelButton.gameObject.SetActive(state.IsCancelButtonVisible);
            commitButton.gameObject.SetActive(state.IsCommitButtonVisible);
            skipButton.gameObject.SetActive(state.IsSkipButtonVisible);
        }
    }
}
 
