using PACG.Core.Characters;
using PACG.Presentation.Cards;
using PACG.Services.Core;
using PACG.Services.ViewModelFactories;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PACG.Presentation.UI.Controllers
{
    /// <summary>
    /// Controller for all of a player's cards (hand, revealed, display, etc.)
    /// </summary>
    public class CardDisplayController : GameBehaviour
    {
        [Header("Player Card Container Transforms")]
        public RectTransform handContainer;
        public RectTransform displayedContainer;
        public RectTransform revealedContainer;
        public RectTransform discardsContainer;

        [Header("Hand Layout")]
        // TODO: Implement hand fanning at large hand sizes
        //public float maxHandWidth = 1200f;
        //public float cardSpacing = 120f;
        //public float fanRadius = 600f;
        //public float hoverHeight = 40f;
        //public AnimationCurve fanCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Card Management")]
        public CardDisplay cardPrefab;

        private PlayerCharacter _pc = null;
        private readonly Dictionary<CardDisplay, CardInstance> displayToInstanceMap = new();
        public CardInstance GetInstanceFromDisplay(CardDisplay display) => displayToInstanceMap.GetValueOrDefault(display, null);

        private readonly Dictionary<CardInstance, CardStagingInfo> originalCardStates = new();

        protected override void OnAwake()
        {
            GameEvents.ActionStaged += OnActionStaged;
            GameEvents.ActionUnstaged += OnActionUnstaged;
        }

        protected void OnDestroy()
        {
            GameEvents.ActionStaged -= OnActionStaged;
            GameEvents.ActionUnstaged -= OnActionUnstaged;
        }

        // ========================================================================================
        // HAND MANAGEMENT
        // ========================================================================================

        public void SetCurrentPC(PlayerCharacter pc)
        {
            if (_pc != null) _pc.HandChanged -= OnHandChanged;
            _pc = pc;
            _pc.HandChanged += OnHandChanged;
        }

        private void OnHandChanged()
        {
            // Clear the hand
            foreach (Transform child in handContainer)
            {
                var cardDisplay = child.GetComponent<CardDisplay>();
                displayToInstanceMap.Remove(cardDisplay);
                Destroy(cardDisplay);
            }

            // Rebuild the hand
            foreach (var card in _pc.Hand) AddCardToHand(card);
        }

        public void AddCardToHand(CardInstance card)
        {
            CardDisplay cardDisplay = Instantiate(cardPrefab, handContainer);
            displayToInstanceMap.Add(cardDisplay, card);
            cardDisplay.SetViewModel(CardViewModelFactory.CreateFrom(card, Contexts.GameContext.AdventureNumber));
            cardDisplay.transform.SetParent(handContainer);
            cardDisplay.transform.localScale = Vector3.one;
        }

        // ========================================================================================
        // ACTION STAGING
        // ========================================================================================

        private void OnActionStaged(IStagedAction action)
        {
            var cardDisplay = FindCardDisplay(action.Card);
            if (cardDisplay == null)
            {
                Debug.LogError($"StageAction --- Unable to find cardDisplay for {action.Card.Data.cardName}. Action not staged!");
                return;
            }

            // If this is the first time we've staged an action for this card, keep track of where the card will be going if undone.
            if (!originalCardStates.ContainsKey(action.Card))
            {
                originalCardStates[action.Card] = new()
                {
                    cardDisplay = cardDisplay,
                    originalParent = cardDisplay.transform.parent,
                    originalScale = cardDisplay.transform.localScale,
                    originalSiblingIndex = cardDisplay.transform.GetSiblingIndex()
                };
            }

            // Hide/move the card based on the action type.
            switch (action.ActionType)
            {
                case PF.ActionType.Display:
                    // Move to display area.
                    cardDisplay.transform.SetParent(displayedContainer);
                    cardDisplay.transform.localScale = new(.6f, .6f);
                    break;
                case PF.ActionType.Draw:
                    cardDisplay.transform.SetParent(handContainer);
                    cardDisplay.transform.localScale = new(1f, 1f);
                    break;
                case PF.ActionType.Reveal:
                    // Move to reveal area.
                    cardDisplay.transform.SetParent(revealedContainer);
                    cardDisplay.transform.localScale = new(.6f, .6f);
                    break;
                default:
                    // Hide the card.
                    cardDisplay.gameObject.SetActive(false);
                    break;
            }
        }

        private void OnActionUnstaged(IStagedAction action)
        {
            if (!originalCardStates.TryGetValue(action.Card, out var stagingInfo))
            {
                Debug.LogError($"Unable to find original card state for {action.Card.Data.cardName}");
                return;
            }

            stagingInfo.cardDisplay.transform.SetParent(stagingInfo.originalParent);
            stagingInfo.cardDisplay.transform.SetSiblingIndex(stagingInfo.originalSiblingIndex);
            stagingInfo.cardDisplay.transform.localScale = stagingInfo.originalScale;
            stagingInfo.cardDisplay.gameObject.SetActive(true);

            originalCardStates.Remove(action.Card);
        }

        private CardDisplay FindCardDisplay(CardInstance card)
        {
            return displayToInstanceMap.FirstOrDefault(kvp => kvp.Value == card).Key;
        }
    }

    public struct CardStagingInfo
    {
        public CardDisplay cardDisplay;
        public Transform originalParent;
        public Vector3 originalScale;
        public int originalSiblingIndex;
    }
}
