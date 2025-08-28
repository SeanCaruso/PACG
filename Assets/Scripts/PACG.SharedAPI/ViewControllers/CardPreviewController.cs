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
        [Header("UI Elements")]
        public GameObject previewArea;
        public Button backgroundButton;

        [Header("Action Buttons")]
        public Transform actionButtonContainer;
        public GameObject actionButtonPrefab;

        private GameObject _currentlyEnlargedCard;

        // Placeholder object with the same size to keep layouts behaving.
        private GameObject _placeholder;
        
        // Original state.
        private Transform _originalParent;
        private int _originalSiblingIndex;
        private Vector3 _originalScale;

        private readonly List<GameObject> _activeActionButtons = new();

        // Dependencies set up via dependency injection in Initialize.
        private ActionStagingManager _actionStagingManager;
        private ContextManager _contexts;

        private void Start()
        {
            // Add a listener to the background button to handle returning the card.
            backgroundButton.onClick.AddListener(ReturnCardToOrigin);
            previewArea.SetActive(false);
            
            GameEvents.TurnStateChanged += ReturnCardToOrigin;
        }

        private void OnDestroy()
        {
            GameEvents.TurnStateChanged -= ReturnCardToOrigin;
        }

        public void Initialize(GameServices gameServices)
        {
            _actionStagingManager = gameServices.ASM;
            _contexts = gameServices.Contexts;
        }

        public void ShowPreviewForCard(GameObject cardObj)
        {
            if (_currentlyEnlargedCard != null) return;

            _currentlyEnlargedCard = cardObj;

            // Store the card's original location and size.
            _originalParent = cardObj.transform.parent;
            _originalSiblingIndex = cardObj.transform.GetSiblingIndex();
            _originalScale = cardObj.transform.localScale;
            
            // Create a placeholder object by cloning the card and making it invisible.
            _placeholder = Instantiate(cardObj.gameObject, cardObj.transform.parent, false);
            _placeholder.name = "Preview Clone";
            
            _placeholder.transform.SetSiblingIndex(cardObj.transform.GetSiblingIndex());
            if (!_placeholder.TryGetComponent<CanvasGroup>(out var placeholderCanvasGroup))
                placeholderCanvasGroup = _placeholder.AddComponent<CanvasGroup>();

            placeholderCanvasGroup.alpha = 0f;

            // Show the preview area.
            previewArea.SetActive(true);

            // Move the card to the preview area and enlarge it.
            var cardRect = cardObj.GetComponent<RectTransform>();
            cardRect.SetParent(previewArea.transform, false);
            cardRect.anchoredPosition = Vector3.zero;
            cardRect.anchorMin = new Vector2(0.5f, 0.5f);
            cardRect.anchorMax = new Vector2(0.5f, 0.5f);
            cardRect.localScale = new Vector3(2f, 2f, 1.0f);

            // Disable any drag handlers.
            if (cardObj.TryGetComponent<CardDragHandler>(out var dragHandler))
            {
                dragHandler.enabled = false;
            }

            if (!cardObj.TryGetComponent<CardDisplay>(out var cardDisplay)) return;

            var cardInstance = cardDisplay.ViewModel.CardInstance;
            if (cardInstance == null)
            {
                Debug.LogWarning($"Tried to preview {cardDisplay.name}, but it has no CardInstance!");
                return;
            }

            // If there's a resolvable, grab any additional actions (damage, give, etc.).
            var playableActions = _contexts.CurrentResolvable?.GetAdditionalActionsForCard(cardInstance) ?? new List<IStagedAction>();
            playableActions.AddRange(cardInstance.GetAvailableActions());

            GenerateActionButtons(playableActions);
        }

        private void GenerateActionButtons(IReadOnlyCollection<IStagedAction> actions)
        {
            foreach (var action in actions)
            {
                var buttonObj = Instantiate(actionButtonPrefab, actionButtonContainer);

                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = action.ActionType.ToString();
                var button = buttonObj.GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    _actionStagingManager.StageAction(action);
                    EndPreview();
                });

                _activeActionButtons.Add(buttonObj);
            }
        }

        private void ReturnCardToOrigin()
        {
            if (!_currentlyEnlargedCard || !_placeholder) return;
            
            // Return the card to its original parent and Z-index.
            _currentlyEnlargedCard.transform.SetParent(_originalParent, false);
            _currentlyEnlargedCard.transform.SetSiblingIndex(_originalSiblingIndex);
            _currentlyEnlargedCard.transform.localScale = _originalScale;

            // Clear action buttons.
            EndPreview();
        }

        private void EndPreview()
        {
            Destroy(_placeholder);
            
            if (!_currentlyEnlargedCard) return;
            
            _currentlyEnlargedCard.transform.localScale = Vector3.one;
            
            // Re-enable any drag handlers.
            if (_currentlyEnlargedCard.TryGetComponent<CardDragHandler>(out var dragHandler))
            {
                dragHandler.enabled = true;
            }

            // Hide the preview and clear the card.
            previewArea.SetActive(false);
            _currentlyEnlargedCard = null;

            // Remove any action buttons.
            foreach (var button in _activeActionButtons)
            {
                Destroy(button);
            }
            _activeActionButtons.Clear();
        }
    }
}
