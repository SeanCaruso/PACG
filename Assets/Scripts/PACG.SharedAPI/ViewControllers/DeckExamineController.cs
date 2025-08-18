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
        [Header("Other Controllers")] public CardDisplayController CardDisplayController;

        [Header("UI Elements")] public GameObject ExamineArea;
        public Button BackgroundButton;
        public Transform CardBacksContainer;
        public Transform CardsContainer;
        public Transform ButtonContainer;

        [Header("Prefabs")] public GameObject ButtonPrefab;
        public GameObject CardBackPrefab;

        // Dependency injections
        private ActionStagingManager _asm;

        // Other members
        private ExamineContext _context;
        private readonly Dictionary<GameObject, CardInstance> _cardDisplayObjectsToInstances = new();
        private CardInstance _topDiscardCardInstance;        

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

        private void OnExamineEvent(ExamineContext context)
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
                var cardDisplay = CardDisplayController.GetCardDisplay(card);
                cardDisplay.transform.SetParent(CardsContainer, worldPositionStays: false);
                
                _cardDisplayObjectsToInstances.Add(cardDisplay.gameObject, card);

                // Add a drag handler (and track the CardInstance) if we can reorder.
                if (context.CanReorder)
                    cardDisplay.gameObject.AddComponent<CardDragHandler>().Initialize(this);
            }
        }
        
        private void ExamineDiscards(ExamineContext context)
        {
            if (CardDisplayController.DiscardsContainer.childCount != 1)
            {
                Debug.LogError($"[{GetType().Name}] Discard pile child count is not 1!");
                return;
            }
            
            // Store the top discard card to show it as the top card in the deck when done.
            var topDiscardDisplay = CardDisplayController.DiscardsContainer.GetChild(0).GetComponent<CardDisplay>();
            
            CardBacksContainer.gameObject.SetActive(false);

            var sortedCards = context.Cards.OrderBy(card => card.ToString());
            foreach (var card in sortedCards)
            {
                var cardDisplay = CardDisplayController.GetCardDisplay(card);
                cardDisplay.transform.SetParent(CardsContainer, worldPositionStays: false);
                _cardDisplayObjectsToInstances.Add(cardDisplay.gameObject, card);
                
                if (topDiscardDisplay == cardDisplay)
                    _topDiscardCardInstance = card;
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
                CardsContainer.GetChild(i).SetParent(CardDisplayController.HiddenContainer);
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

            if (_context?.ExamineMode == ExamineContext.Mode.Discard && _topDiscardCardInstance != null)
                CardDisplayController.OnCardLocationChanged(_topDiscardCardInstance);

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
