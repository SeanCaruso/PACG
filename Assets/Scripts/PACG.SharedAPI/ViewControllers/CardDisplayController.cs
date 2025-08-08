using PACG.Gameplay;
using PACG.Presentation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PACG.SharedAPI
{
    /// <summary>
    /// Controller for all visible, non-previewed cards (hour, explored, hand, revealed, display, etc.)
    /// </summary>
    public class CardDisplayController : MonoBehaviour
    {
        [Header("Non-Player Card Container Transforms")]
        public RectTransform hoursContainer;
        public RectTransform encounteredContainer;

        [Header("Player Card Container Transforms")]
        public RectTransform handContainer;
        public RectTransform displayedContainer;
        public RectTransform revealedContainer;
        public RectTransform discardsContainer;
        public RectTransform recoveryContainer;
        public RectTransform hiddenContainer;

        [Header("Hand Layout")]
        // TODO: Implement hand fanning at large hand sizes
        //public float maxHandWidth = 1200f;
        //public float cardSpacing = 120f;
        //public float fanRadius = 600f;
        //public float hoverHeight = 40f;
        //public AnimationCurve fanCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Card Management")]
        public CardDisplay cardPrefab;

        private PlayerCharacter PC { get; set; } = null;

        private readonly Dictionary<CardInstance, CardDisplay> instanceToDisplayMap = new();
        public CardInstance GetInstanceFromDisplay(CardDisplay display) => instanceToDisplayMap.FirstOrDefault(kvp => kvp.Value == display).Key;

        protected void Awake()
        {
            GameEvents.HourChanged += OnHourChanged;
            GameEvents.EncounterStarted += OnEncounterStarted;

            GameEvents.CardLocationChanged += OnCardLocationChanged;
            GameEvents.CardLocationsChanged += OnCardLocationsChanged;
        }

        protected void OnDestroy()
        {
            GameEvents.HourChanged -= OnHourChanged;
            GameEvents.EncounterStarted -= OnEncounterStarted;

            GameEvents.CardLocationChanged -= OnCardLocationChanged;
            GameEvents.CardLocationsChanged -= OnCardLocationsChanged;
        }

        // ========================================================================================
        // TURN AND ENCOUNTER MANAGEMENT
        // ========================================================================================

        public void OnHourChanged(CardInstance hourCard)
        {
            CardDisplay cardDisplay = Instantiate(cardPrefab, hoursContainer);
            instanceToDisplayMap.Add(hourCard, cardDisplay);
            cardDisplay.SetViewModel(CardViewModelFactory.CreateFrom(hourCard));
            //cardDisplay.transform.localScale = Vector3.one;
        }

        public void OnEncounterStarted(CardInstance encounteredCard)
        {
            CardDisplay cardDisplay = Instantiate(cardPrefab, encounteredContainer);
            instanceToDisplayMap.Add(encounteredCard, cardDisplay);
            cardDisplay.SetViewModel(CardViewModelFactory.CreateFrom(encounteredCard));
            cardDisplay.transform.localScale = Vector3.one;
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

        private void OnCardLocationChanged(CardInstance card)
        {
            var cardDisplay = GetCardDisplay(card);
            if (cardDisplay == null)
            {
                Debug.LogError($"StageAction --- Unable to find cardDisplay for {card.Data.cardName}. Action not staged!");
                return;
            }

            if (card.Owner != PC) return;

            // Hide/move the card based on the action type.
            switch (card.CurrentLocation)
            {
                // Locations where the card is hidden
                case CardLocation.Buried:
                case CardLocation.Deck:
                case CardLocation.Discard: // TODO: Display last discarded card in discard pile?
                case CardLocation.Vault:
                    cardDisplay.transform.SetParent(hiddenContainer);
                    break;
                case CardLocation.Displayed:
                    // Move to display area.
                    cardDisplay.transform.SetParent(displayedContainer);
                    cardDisplay.transform.localScale = new(.6f, .6f);
                    break;
                case CardLocation.Hand:
                    cardDisplay.transform.SetParent(handContainer);
                    cardDisplay.transform.localScale = new(1f, 1f);
                    break;
                case CardLocation.Recovery:
                    cardDisplay.transform.SetParent(recoveryContainer);
                    cardDisplay.transform.localScale = new(.6f, .6f);
                    break;
                case CardLocation.Revealed:
                    // Move to reveal area.
                    cardDisplay.transform.SetParent(revealedContainer);
                    cardDisplay.transform.localScale = new(.6f, .6f);
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

        private CardDisplay GetCardDisplay(CardInstance card)
        {
            // If it already exists, return it.
            if (instanceToDisplayMap.TryGetValue(card, out var display)) return display;

            // Otherwise, create a new one.
            CardDisplay cardDisplay = Instantiate(cardPrefab);
            cardDisplay.SetViewModel(CardViewModelFactory.CreateFrom(card));
            instanceToDisplayMap.Add(card, cardDisplay);
            return cardDisplay;
        }
    }
}
