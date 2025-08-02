using PACG.Presentation.Cards;
using PACG.Services.Core;
using PACG.SharedAPI.PresentationContexts;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PACG.Presentation.UI.Controllers
{
    public class CardPreviewController : GameBehaviour
    {
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

        private void Start()
        {
            // Add a listener to the background button to handle returning the card.
            backgroundButton.onClick.AddListener(ReturnCardToOrigin);
            previewArea.SetActive(false);
        }

        public void ShowPreviewForCard(CardDisplay cardDisplay)
        {
            if (currentlyEnlargedCard != null)
            {
                Debug.LogError($"Tried to preview {cardDisplay.name}, but {currentlyEnlargedCard.name} is already being previewed!");
                return;
            }

            var cardInstance = ServiceLocator.Get<CardDisplayController>().GetInstanceFromDisplay(cardDisplay);
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
            GenerateActionButtons(Logic.GetPlayableLogic(cardInstance).GetAvailableActions());
        }

        public void GenerateActionButtons(IReadOnlyCollection<IStagedAction> actions)
        {
            foreach (var action in actions)
            {
                GameObject buttonObj = Instantiate(actionButtonPrefab, actionButtonContainer);

                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = action.ToString();
                Button button = buttonObj.GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    ServiceLocator.Get<ActionStagingManager>().StageAction(action);
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
