using PACG.Gameplay;
using PACG.Presentation;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PACG.SharedAPI
{
    public class CardPreviewController : MonoBehaviour
    {
        [Header("Other Controllers")]
        public CardDisplayController cardDisplayController;

        [Header("UI Elements")]
        public GameObject previewArea;
        public Button backgroundButton;

        [Header("Action Buttons")]
        public Transform actionButtonContainer;
        public GameObject actionButtonPrefab;

        private CardDisplay currentlyEnlargedCard;
        private Transform originalParent;
        private int originalSiblingIndex;
        private Vector3 originalScale;

        private readonly List<GameObject> activeActionButtons = new();

        // Dependencies set up via dependency injection in Initialize.
        private LogicRegistry _logic;
        private ActionStagingManager _actionStagingManager;

        private void Start()
        {
            // Add a listener to the background button to handle returning the card.
            backgroundButton.onClick.AddListener(ReturnCardToOrigin);
            previewArea.SetActive(false);
        }

        public void Initialize(GameServices gameServices)
        {
            _logic = gameServices.Logic;
            _actionStagingManager = gameServices.ASM;
        }

        public void ShowPreviewForCard(CardDisplay cardDisplay)
        {
            if (currentlyEnlargedCard != null) return;

            var cardInstance = cardDisplayController.GetInstanceFromDisplay(cardDisplay);
            if (cardInstance == null)
            {
                Debug.LogError($"Tried to preview {cardDisplay.name}, but it has no CardInstance!");
                return;
            }

            currentlyEnlargedCard = cardDisplay;

            // Store the card's original location and size.
            originalParent = cardDisplay.transform.parent;
            originalSiblingIndex = cardDisplay.transform.GetSiblingIndex();
            originalScale = cardDisplay.transform.localScale;

            // Show the preview area.
            previewArea.SetActive(true);

            // Move the card to the preview area and enlarge it.
            var cardRect = cardDisplay.GetComponent<RectTransform>();
            cardRect.SetParent(previewArea.transform, false);
            cardRect.anchoredPosition = Vector3.zero;
            cardRect.anchorMin = new(0.5f, 0.5f);
            cardRect.anchorMax = new(0.5f, 0.5f);
            cardRect.localScale = new Vector3(2f, 2f, 1.0f);

            // Query the card logic for any playable actions.
            GenerateActionButtons(_logic.GetPlayableLogic(cardInstance)?.GetAvailableActions() ?? new());
        }

        public void GenerateActionButtons(IReadOnlyCollection<IStagedAction> actions)
        {
            foreach (var action in actions)
            {
                GameObject buttonObj = Instantiate(actionButtonPrefab, actionButtonContainer);

                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = action.ActionType.ToString();
                Button button = buttonObj.GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    _actionStagingManager.StageAction(action);
                    EndPreview();
                });

                activeActionButtons.Add(buttonObj);
            }
        }

        private void ReturnCardToOrigin()
        {
            if (currentlyEnlargedCard == null) return;

            // Return the card to its original parent and Z-index.
            currentlyEnlargedCard.transform.SetParent(originalParent, false);
            currentlyEnlargedCard.transform.SetSiblingIndex(originalSiblingIndex);
            currentlyEnlargedCard.transform.localScale = originalScale;

            // Clear action buttons.
            EndPreview();
        }

        private void EndPreview()
        {
            // Hide the preview and clear the card.
            previewArea.SetActive(false);
            currentlyEnlargedCard = null;

            // Remove any action buttons.
            foreach (var button in activeActionButtons)
            {
                Destroy(button);
            }
            activeActionButtons.Clear();
        }
    }
}
