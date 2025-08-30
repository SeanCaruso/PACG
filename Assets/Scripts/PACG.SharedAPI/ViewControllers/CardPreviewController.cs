using System.Collections.Generic;
using PACG.Gameplay;
using PACG.Presentation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PACG.SharedAPI
{
    public class CardPreviewController : MonoBehaviour
    {
        [Header("Dependencies")]
        public CardDisplayFactory CardDisplayFactory;

        [Header("UI Elements")]
        public GameObject PreviewArea;
        public Transform PreviewContainer;
        public Button BackgroundButton;

        [Header("Action Buttons")]
        public Transform ActionButtonContainer;
        public GameObject ActionButtonPrefab;

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
            PreviewArea.SetActive(false);

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
            if (_currentlyEnlargedCard) return;

            BackgroundButton.onClick.AddListener(ReturnCardToOrigin);

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
            PreviewArea.SetActive(true);

            // Move the card to the preview area and enlarge it.
            var cardRect = cardObj.GetComponent<RectTransform>();
            cardRect.SetParent(PreviewContainer, false);
            cardRect.anchoredPosition = Vector3.zero;
            cardRect.anchorMin = new Vector2(0.5f, 0.5f);
            cardRect.anchorMax = new Vector2(0.5f, 0.5f);

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

            var playableActions = new List<IStagedAction>();
            // If there's an encountered card, grab any additional actions.
            playableActions.AddRange(_contexts.EncounterContext?.Card?.GetAdditionalActionsForCard(cardInstance) ??
                                     new List<IStagedAction>());

            // If there's a resolvable, grab any additional actions (damage, give, etc.).
            playableActions.AddRange(_contexts.CurrentResolvable?.GetAdditionalActionsForCard(cardInstance) ??
                                     new List<IStagedAction>());
            playableActions.AddRange(cardInstance.GetAvailableActions());

            GenerateActionButtons(playableActions);
        }

        private void GenerateActionButtons(IReadOnlyCollection<IStagedAction> actions)
        {
            foreach (var action in actions)
            {
                var buttonObj = Instantiate(ActionButtonPrefab, ActionButtonContainer);

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
            BackgroundButton.onClick.RemoveAllListeners();

            if (!_currentlyEnlargedCard) return;

            _currentlyEnlargedCard.transform.localScale = Vector3.one;

            // Re-enable any drag handlers.
            if (_currentlyEnlargedCard.TryGetComponent<CardDragHandler>(out var dragHandler))
            {
                dragHandler.enabled = true;
            }

            // Hide the preview and clear the card.
            PreviewArea.SetActive(false);
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
