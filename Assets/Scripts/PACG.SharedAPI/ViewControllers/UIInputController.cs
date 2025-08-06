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
        private TurnManager _turnManager;

        protected void Start()
        {
            // Subscribe to events.
            GameEvents.TurnStateChanged += UpdateTurnButtons;

            giveCardButton.onClick.AddListener(() => GiveCardButton_OnClick());
            moveButton.onClick.AddListener(() => MoveButton_OnClick());
            exploreButton.onClick.AddListener(() => ExploreButton_OnClick());
            optionalDiscardButton.onClick.AddListener(() =>  OptionalDiscardButton_OnClick());
            endTurnButton.onClick.AddListener(() => EndTurnButton_OnClick());
        }

        protected void OnDestroy()
        {
            GameEvents.TurnStateChanged -= UpdateTurnButtons;
        }

        public void Initialize(TurnManager turnManager)
        {
            _turnManager = turnManager;
        }

        // --- Turn Flow -----------------------------------------

        public void GiveCardButton_OnClick() => _turnManager.GiveCard();
        public void MoveButton_OnClick() => _turnManager.MoveToLocation();
        public void ExploreButton_OnClick() => _turnManager.Explore();
        public void OptionalDiscardButton_OnClick() => _turnManager.OptionalDiscards();
        public void EndTurnButton_OnClick() => _turnManager.EndTurn();

        protected void UpdateTurnButtons()
        {
            giveCardButton.enabled = _turnManager.CanGive;
            moveButton.enabled = _turnManager.CanMove;
            exploreButton.enabled = _turnManager.CanExplore;
            optionalDiscardButton.enabled = _turnManager.CurrentPC.Hand.Count > 0;
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
 
