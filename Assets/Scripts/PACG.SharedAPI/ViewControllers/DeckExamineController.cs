using PACG.Gameplay;
using PACG.Presentation;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PACG.SharedAPI
{
    public class ExamineContext
    {
        public enum Mode
        {
            Deck,
            Discard
        }

        public Mode ExamineMode { get; set; } = Mode.Deck;
        public List<CardInstance> Cards { get; set; } = new();
        public int UnknownCount { get; set; }
        public bool CanReorder { get; set; }
        public System.Action OnClose { get; set; }
    }

    public class DeckExamineController : MonoBehaviour
    {
        [Header("Other Dependencies")]
        public CardDisplayFactory CardDisplayFactory;

        [Header("UI Elements")]
        public GameObject ExamineArea;
        public Button BackgroundButton;
        public Transform CardBacksContainer;
        public Transform CardsContainer;
        public Transform ButtonContainer;

        [Header("Prefabs")]
        public GameObject ButtonPrefab;
        public GameObject CardBackPrefab;

        // Dependency injections
        private ActionStagingManager _asm;

        // Other members
        private ExamineContext _context;
        private readonly Dictionary<GameObject, CardInstance> _cardDisplayObjectsToInstances = new();

        protected void Awake()
        {
            DialogEvents.ExamineEvent += OnExamineEvent;
        }

        protected void OnDestroy()
        {
            DialogEvents.ExamineEvent -= OnExamineEvent;
        }

        protected void OnEnable()
        {
            BackgroundButton.onClick.AddListener(EndExamine);
        }

        protected void OnDisable()
        {
            BackgroundButton.onClick.RemoveAllListeners();
        }

        public void Initialize(GameServices gameServices)
        {
            _asm = gameServices.ASM;
        }

        public void OnExamineEvent(ExamineContext context)
        {
            _context = context;

            // Clears any old states.
            PrepareExamine();

            ExamineArea.SetActive(true);

            switch (context.ExamineMode)
            {
                case ExamineContext.Mode.Deck:
                    ExamineDeck(context);
                    break;
                case ExamineContext.Mode.Discard:
                    ExamineDiscards(context);
                    break;
            }

            var buttonObj = Instantiate(ButtonPrefab, ButtonContainer);
            buttonObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = "Continue";
            buttonObj.GetComponent<Button>().onClick.AddListener(EndExamine);
        }

        private void ExamineDeck(ExamineContext context)
        {
            CardBacksContainer.gameObject.SetActive(true);

            for (var i = 0; i < context.UnknownCount; i++)
            {
                Instantiate(CardBackPrefab, CardBacksContainer);
            }

            foreach (var card in context.Cards)
            {
                var cardDisplay = CardDisplayFactory.CreateCardDisplay(
                    card,
                    CardDisplayFactory.DisplayContext.Browser,
                    CardsContainer
                );

                _cardDisplayObjectsToInstances.Add(cardDisplay.gameObject, card);

                // Enable/disable drag handler as needed.
                if (!cardDisplay.TryGetComponent<CardDragHandler>(out var dragHandler)) continue;
                dragHandler.Initialize(this);
                dragHandler.enabled = context.CanReorder;
            }
        }

        private void ExamineDiscards(ExamineContext context)
        {
            CardBacksContainer.gameObject.SetActive(false);

            var sortedCards = context.Cards.OrderBy(card => card.ToString());
            foreach (var card in sortedCards)
            {
                var cardDisplay = CardDisplayFactory.CreateCardDisplay(
                    card,
                    CardDisplayFactory.DisplayContext.Browser,
                    CardsContainer
                );
                if (cardDisplay.TryGetComponent<CardDragHandler>(out var dragHandler))
                    dragHandler.enabled = false;
                
                _cardDisplayObjectsToInstances.Add(cardDisplay.gameObject, card);
            }
        }

        private void PrepareExamine()
        {
            for (var i = CardBacksContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(CardBacksContainer.GetChild(i).gameObject);
            }

            for (var i = CardsContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(CardsContainer.GetChild(i).gameObject);
            }

            if (ButtonContainer.childCount == 1)
                Destroy(ButtonContainer.GetChild(0).gameObject);
        }

        private void EndExamine()
        {
            foreach (var cardDisplay in _cardDisplayObjectsToInstances.Keys)
            {
                Destroy(cardDisplay.GetComponent<CardDragHandler>());
            }

            _cardDisplayObjectsToInstances.Clear();

            _context?.OnClose?.Invoke();
            ExamineArea.SetActive(false);

            if (_context?.ExamineMode == ExamineContext.Mode.Deck)
                _asm.Commit();
        }

        public void SwapCards(GameObject cardA, GameObject cardB)
        {
            if (_context == null) return;

            if (!_context.CanReorder) return;

            // Swap visual positions.
            var indexA = cardA.transform.GetSiblingIndex();
            var indexB = cardB.transform.GetSiblingIndex();

            cardA.transform.SetSiblingIndex(indexB);
            cardB.transform.SetSiblingIndex(indexA);

            // Swap instances in the CurrentOrder list.
            var instanceA = _cardDisplayObjectsToInstances[cardA];
            var instanceB = _cardDisplayObjectsToInstances[cardB];

            var listIndexA = _context.Cards.IndexOf(instanceA);
            var listIndexB = _context.Cards.IndexOf(instanceB);

            if (listIndexA < 0 || listIndexB < 0)
                return;

            Debug.Log($"[{GetType().Name}] Swapping {instanceA} with {instanceB}");

            (_context.Cards[listIndexA], _context.Cards[listIndexB]) =
                (_context.Cards[listIndexB], _context.Cards[listIndexA]);
        }
    }
}
