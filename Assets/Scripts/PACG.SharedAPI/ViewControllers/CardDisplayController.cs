using PACG.Gameplay;
using PACG.Presentation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PACG.SharedAPI
{
    /// <summary>
    /// Controller for all visible, non-previewed cards (hour, explored, hand, revealed, display, etc.)
    /// </summary>
    public class CardDisplayController : MonoBehaviour
    {
        [Header("Non-Player Card Container Transforms")]
        public RectTransform HoursContainer;
        public RectTransform EncounteredContainer;

        [Header("Player Card Container Transforms")]
        public RectTransform HandContainer;
        public RectTransform DisplayedContainer;
        public RectTransform RevealedContainer;
        public RectTransform DiscardsContainer;
        public RectTransform RecoveryContainer;
        public RectTransform HiddenContainer;

        [Header("Hand Layout")]
        // TODO: Implement hand fanning at large hand sizes
        //public float maxHandWidth = 1200f;
        //public float cardSpacing = 120f;
        //public float fanRadius = 600f;
        //public float hoverHeight = 40f;
        //public AnimationCurve fanCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Card Management")]
        public CardDisplay CardPrefab;

        private PlayerCharacter PC { get; set; }

        private readonly Dictionary<CardInstance, CardDisplay> _instanceToDisplayMap = new();
        public CardInstance GetInstanceFromDisplay(CardDisplay display) => _instanceToDisplayMap.FirstOrDefault(kvp => kvp.Value == display).Key;

        protected void Awake()
        {
            SetupDiscardClickHandler();
            
            GameEvents.HourChanged += OnHourChanged;
            GameEvents.EncounterStarted += OnEncounterStarted;
            GameEvents.EncounterEnded += OnEncounterEnded;

            GameEvents.CardLocationChanged += OnCardLocationChanged;
            GameEvents.CardLocationsChanged += OnCardLocationsChanged;
        }

        protected void OnDestroy()
        {
            GameEvents.HourChanged -= OnHourChanged;
            GameEvents.EncounterStarted -= OnEncounterStarted;
            GameEvents.EncounterEnded -= OnEncounterEnded;

            GameEvents.CardLocationChanged -= OnCardLocationChanged;
            GameEvents.CardLocationsChanged -= OnCardLocationsChanged;
        }

        // ========================================================================================
        // TURN AND ENCOUNTER MANAGEMENT
        // ========================================================================================

        private void OnHourChanged(CardInstance hourCard)
        {
            var cardDisplay = GetCardDisplay(hourCard);
            cardDisplay.transform.SetParent(HoursContainer, worldPositionStays: false);
            cardDisplay.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        private void OnEncounterStarted(CardInstance encounteredCard)
        {
            var cardDisplay = GetCardDisplay(encounteredCard);
            cardDisplay.transform.SetParent(EncounteredContainer, worldPositionStays: false);
            cardDisplay.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        private void OnEncounterEnded()
        {
            for (var i = EncounteredContainer.childCount - 1; i >= 0; i--)
            {
                EncounteredContainer.GetChild(i).SetParent(HiddenContainer);
            }
        }

        // ========================================================================================
        // HAND MANAGEMENT
        // ========================================================================================

        public void SetCurrentPC(PlayerCharacter pc)
        {
            PC = pc;
        }

        // ========================================================================================
        // CARD MOVEMENT
        // ========================================================================================

        public void OnCardLocationChanged(CardInstance card)
        {
            var cardDisplay = GetCardDisplay(card);
            if (!cardDisplay)
            {
                Debug.LogError($"StageAction --- Unable to find cardDisplay for {card.Data.cardName}. Action not staged!");
                return;
            }

            if (card.Owner != PC) return;
            
            ResetCardDisplay(cardDisplay);

            // Hide/move the card based on the action type.
            switch (card.CurrentLocation)
            {
                // Locations where the card is hidden
                case CardLocation.Buried:
                case CardLocation.Deck:
                case CardLocation.Vault:
                    cardDisplay.transform.SetParent(HiddenContainer);
                    break;
                case CardLocation.Discard:
                    ShowLastDiscardedCard(cardDisplay);
                    break;
                case CardLocation.Displayed:
                    cardDisplay.transform.SetParent(DisplayedContainer);
                    cardDisplay.transform.localScale = new Vector3(.6f, .6f);
                    break;
                case CardLocation.Hand:
                    cardDisplay.transform.SetParent(HandContainer);
                    break;
                case CardLocation.Recovery:
                    cardDisplay.transform.SetParent(RecoveryContainer);
                    cardDisplay.transform.localScale = new Vector3(.6f, .6f);
                    break;
                case CardLocation.Revealed:
                    cardDisplay.transform.SetParent(RevealedContainer);
                    cardDisplay.transform.localScale = new Vector3(.6f, .6f);
                    break;
                default:
                    Debug.LogError($"[CardDisplayController] Unknown card location: {card.CurrentLocation} for {card.Data.cardName}");
                    break;
            }
        }

        private void OnCardLocationsChanged(List<CardInstance> cards)
        {
            foreach (var card in cards) OnCardLocationChanged(card);
        }

        private void ShowLastDiscardedCard(CardDisplay cardDisplay)
        {
            if (DiscardsContainer.childCount == 1)
            {
                var previousDiscard = DiscardsContainer.GetChild(0);
                previousDiscard.SetParent(HiddenContainer);
                if (previousDiscard.TryGetComponent<CanvasGroup>(out var canvasGroup))
                    canvasGroup.blocksRaycasts = true;
            }

            cardDisplay.transform.SetParent(DiscardsContainer);
            if (!cardDisplay.TryGetComponent<CanvasGroup>(out var discardGroup))
                discardGroup = cardDisplay.gameObject.AddComponent<CanvasGroup>();
            discardGroup.blocksRaycasts = false;
        }

        /// <summary>
        /// Resets the card display to its original scale, block raycasts, and without a drag handler.
        /// </summary>
        /// <param name="cardDisplay"></param>
        private static void ResetCardDisplay(CardDisplay cardDisplay)
        {
            cardDisplay.transform.localScale = Vector3.one;
            
            if (cardDisplay.TryGetComponent<CanvasGroup>(out var canvasGroup))
                canvasGroup.blocksRaycasts = true;
            
            if (cardDisplay.TryGetComponent<CardDragHandler>(out var dragHandler))
                Destroy(dragHandler);
        }

        private void HideCard(CardInstance card) => GetCardDisplay(card).transform.SetParent(HiddenContainer);
        public void HideCard(CardDisplay card) => HideCard(GetInstanceFromDisplay(card));
        
        // ========================================================================================
        // DECK EXAMINATION
        // ========================================================================================
        private void SetupDiscardClickHandler()
        {
            if (!DiscardsContainer.TryGetComponent<Button>(out var discardPileButton))
                discardPileButton = DiscardsContainer.gameObject.AddComponent<Button>();
            discardPileButton.onClick.AddListener(OnDiscardClicked);
        }

        private void OnDiscardClicked()
        {
            if (PC == null || PC.Discards.Count == 0) return;
            
            if (DiscardsContainer.childCount != 1)
            {
                Debug.LogError($"[CardDisplayController] Discard pile child count is not 1!");
                return;
            }

            var context = new ExamineContext
            {
                ExamineMode = ExamineContext.Mode.Discard,
                Cards = PC.Discards.ToList(),
                UnknownCount = 0,
                CanReorder = false,
                OnClose = () => { }
            };
            
            DialogEvents.RaiseExamineEvent(context);
        }

        // ========================================================================================
        // CardDisplay INSTANTIATION
        // ========================================================================================
        public CardDisplay GetCardDisplay(CardInstance card)
        {
            // If it already exists, return it.
            if (_instanceToDisplayMap.TryGetValue(card, out var display)) return display;

            // Otherwise, create a new one.
            var cardDisplay = Instantiate(CardPrefab, HiddenContainer);
            cardDisplay.SetViewModel(CardViewModelFactory.CreateFrom(card));
            _instanceToDisplayMap.Add(card, cardDisplay);
            return cardDisplay;
        }
    }
}
