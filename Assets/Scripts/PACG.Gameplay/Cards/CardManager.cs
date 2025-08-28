using System.Collections.Generic;
using System.Linq;
using PACG.Core;
using PACG.Data;
using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    public class CardManager
    {
        private LogicRegistry _logic;

        private readonly List<CardInstance> _allCards = new();
        
        private readonly ResponseRegistry _responseRegistry = new();

        public void Initialize(GameServices gameServices)
        {
            _logic = gameServices.Logic;
        }

        public CardInstance New(CardData card, PlayerCharacter owner = null)
        {
            if (card == null)
            {
                Debug.LogError("Cannot create a card from null CardData!");
                return null;
            }

            CardInstance newInstance = new(card, _logic.GetLogic<CardLogicBase>(card.cardID), owner);
            _allCards.Add(newInstance);

            return newInstance;
        }

        public void TriggerBeforeDiscard(DiscardEventArgs args) => _responseRegistry.TriggerBeforeDiscard(args);

        public void MoveCard(CardInstance card, CardLocation newLocation)
        {
            if (card == null)
            {
                Debug.LogWarning($"Attempted to move null card to {newLocation}!");
                return;
            }

            if (card.CurrentLocation == newLocation)
            {
                return;
            }
            
            var wasActive = card.CurrentLocation is (CardLocation.Hand or CardLocation.Revealed or CardLocation.Displayed);
            var isActive = newLocation is (CardLocation.Hand or CardLocation.Revealed or CardLocation.Displayed);
            
            switch (isActive)
            {
                case true when !wasActive:
                    _responseRegistry.RegisterResponses(card);
                    break;
                case false when wasActive:
                    _responseRegistry.UnregisterResponses(card);
                    break;
            }

            card.CurrentLocation = newLocation;
            GameEvents.RaiseCardLocationChanged(card);

            if (card.Owner != null)
                GameEvents.RaisePlayerDeckCountChanged(card.Owner.DeckCards.Count);
        }

        /// <summary>
        /// Convenience function to handle cards moved by PlayerCharacter actions.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="action"></param>
        public void MoveCard(CardInstance card, ActionType action)
        {
            if (card == null)
            {
                Debug.LogWarning($"[{GetType().Name}] Attempted to move null card!");
                return;
            }
            
            if (card.Owner == null)
            {
                Debug.LogWarning($"[{GetType().Name}] {card.Data.cardName} has no owner - use MoveCard(CardInstance, CardLocation) instead!");
                return;
            }

            switch (action)
            {
                case ActionType.Banish:
                    MoveCard(card, card.OriginalOwner == null ? CardLocation.Vault : CardLocation.Recovery);
                    break;
                case ActionType.Bury:
                    MoveCard(card, CardLocation.Buried);
                    break;
                case ActionType.Discard:
                    MoveCard(card, CardLocation.Discard);
                    break;
                case ActionType.Display:
                    MoveCard(card, CardLocation.Displayed);
                    break;
                case ActionType.Draw:
                    MoveCard(card, CardLocation.Hand);
                    break;
                case ActionType.Recharge:
                    MoveCard(card, CardLocation.Deck);
                    card.Owner.Recharge(card);
                    break;
                case ActionType.Reload:
                    MoveCard(card, CardLocation.Deck);
                    card.Owner.Reload(card);
                    break;
                case ActionType.Reveal:
                    MoveCard(card, CardLocation.Revealed);
                    break;
                default:
                    Debug.LogError($"Unsupported action: {action}!");
                    break;
            }
        }

        public void RestoreRevealedCardsToHand()
        {
            var revealedCards = GetCardsInLocation(CardLocation.Revealed);
            foreach (var card in revealedCards)
            {
                MoveCard(card, CardLocation.Hand);
            }
        }

        // Find cards...
        private List<CardInstance> FindAll(System.Func<CardInstance, bool> predicate) => _allCards.Where(predicate).ToList();
        // ... by location
        public List<CardInstance> GetCardsInLocation(CardLocation location) => FindAll(card => card.CurrentLocation == location);
        // ... owned by a specific player
        public List<CardInstance> GetCardsOwnedBy(PlayerCharacter owner) => FindAll(card => card.Owner == owner);
        // ... owned by a specific player in a specific location
        public List<CardInstance> GetCardsOwnedBy(PlayerCharacter owner, CardLocation location) => FindAll(card => card.Owner == owner && card.CurrentLocation == location);
        // ... are considered part of a specific player's hand (in hand and revealed)
        public List<CardInstance> GetCardsInHand(PlayerCharacter owner) => FindAll(card => card.Owner == owner && card.CurrentLocation is CardLocation.Hand or CardLocation.Revealed);
    }
}
