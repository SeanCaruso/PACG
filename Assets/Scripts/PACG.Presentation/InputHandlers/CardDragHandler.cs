using PACG.SharedAPI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PACG.Presentation
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private CardInputHandler _cardInputHandler;
        private Canvas _rootCanvas;

        // Placeholder object with the same size to keep layouts behaving.
        private GameObject _placeholder;

        // Original state
        private Transform _originalParent;
        private Vector3 _originalScale;
        private int _originalSiblingIndex;

        // Dependency injection via Initialize
        private DeckExamineController _examineController;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _cardInputHandler = GetComponent<CardInputHandler>();
            _rootCanvas = GetComponentInParent<Canvas>();
        }

        public void Initialize(DeckExamineController examineController)
        {
            _examineController = examineController;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_rootCanvas == null) return;

            // ====================================================================================
            // Create a placeholder object by cloning the card and making it non-interactable.
            // ====================================================================================
            _placeholder = Instantiate(gameObject, transform.parent, false);
            _placeholder.name = "Drag Clone";

            _placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());

            Destroy(_placeholder.GetComponent<CardDragHandler>());
            if (!_placeholder.TryGetComponent<CanvasGroup>(out var placeholderCanvasGroup))
                placeholderCanvasGroup = _placeholder.AddComponent<CanvasGroup>();

            placeholderCanvasGroup.alpha = 0f;
            placeholderCanvasGroup.blocksRaycasts = false;

            // ====================================================================================
            // Store original state and reparent the card.
            // ====================================================================================
            _originalParent = transform.parent;
            _originalSiblingIndex = transform.GetSiblingIndex();
            _originalScale = transform.localScale;

            transform.SetParent(_rootCanvas.transform, true);
            transform.SetAsLastSibling();

            // ====================================================================================
            // Prepare for drag.
            // ====================================================================================
            if (_cardInputHandler != null)
                _cardInputHandler.enabled = false;

            _canvasGroup.blocksRaycasts = false;
            transform.localScale = _originalScale * 1.1f;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_rootCanvas == null) return;

            // Convert mouse position to a local point in the canvas.
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rootCanvas.transform as RectTransform,
                eventData.position,
                _rootCanvas.worldCamera,
                out Vector2 localPoint
            );
            _rectTransform.localPosition = localPoint;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_placeholder == null) return;

            // Return the card and destroy the placeholder.
            Destroy(_placeholder);

            transform.SetParent(_originalParent, true);
            transform.SetSiblingIndex(_originalSiblingIndex);
            transform.localScale = _originalScale;
            _canvasGroup.blocksRaycasts = true;

            if (_cardInputHandler != null)
                _cardInputHandler.enabled = true;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (!_examineController)
            {
                Debug.LogError($"[{GetType().Name}] Examine controller is null - CardDragHandler was not initialized!");
                return;
            }
            
            var draggedObject = eventData.pointerDrag;
            if (draggedObject && draggedObject != gameObject)
            {
                _examineController.SwapCards(gameObject, draggedObject);
            }
        }
    }
}
