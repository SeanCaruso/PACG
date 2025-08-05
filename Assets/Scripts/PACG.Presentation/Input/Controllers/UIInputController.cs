using PACG.Gameplay;
using PACG.SharedAPI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace PACG.Presentation
{
    public class UIInputController : GameBehaviour
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

        public static UIInputController Instance { get; private set; }

        private TurnManager turnManager;

        protected void Start()
        {
            // Subscribe to events.
            GameEvents.TurnPhaseChanged += UpdateTurnButtons;

            turnManager = ServiceLocator.Get<TurnManager>();
            if (turnManager == null) Debug.LogError("UIInputController can't find TurnManager!");

            giveCardButton.onClick.AddListener(() => GiveCardButton_OnClick());
            moveButton.onClick.AddListener(() => MoveButton_OnClick());
            exploreButton.onClick.AddListener(() => ExploreButton_OnClick());
            optionalDiscardButton.onClick.AddListener(() =>  OptionalDiscardButton_OnClick());
            endTurnButton.onClick.AddListener(() => EndTurnButton_OnClick());
        }

        protected void OnDestroy()
        {
            GameEvents.TurnPhaseChanged -= UpdateTurnButtons;
        }

        // --- Turn Flow -----------------------------------------

        public void GiveCardButton_OnClick() => turnManager.GiveCard();
        public void MoveButton_OnClick() => turnManager.MoveToLocation();
        public void ExploreButton_OnClick() => turnManager.Explore();
        public void OptionalDiscardButton_OnClick() => turnManager.OptionalDiscards();
        public void EndTurnButton_OnClick() => turnManager.EndTurn();

        protected void UpdateTurnButtons()
        {
            giveCardButton.enabled = turnManager.CanGive;
            moveButton.enabled = turnManager.CanMove;
            exploreButton.enabled = turnManager.CanExplore;
            optionalDiscardButton.enabled = Contexts.TurnContext.CurrentPC.Hand.Count > 0;
        }

        // --- Action Staging Flow -----------------------------------

        public event Action OnCancelButtonClicked;
        public void CancelButton_OnClick() => OnCancelButtonClicked?.Invoke();

        public event Action OnCommitButtonClicked;
        public void CommitButton_OnClick() => OnCommitButtonClicked?.Invoke();

        public event Action OnSkipButtonClicked;
        public void SkipButton_OnClick() => OnSkipButtonClicked?.Invoke();
    }
}
 
