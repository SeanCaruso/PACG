using PACG.SharedAPI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PACG.Gameplay
{
    public class CardManager
    {
        private LogicRegistry _logic;

        private readonly List<CardInstance> allCards = new();
        private readonly List<CardInstance> theVault = new();

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

            CardInstance newInstance = new(card, _logic.GetCardLogic(card.cardID), owner);
            allCards.Add(newInstance);

            return newInstance;
        }

        public void MoveCard(CardInstance card, CardLocation newLocation)
        {
            if (card == null)
            {
                Debug.LogError($"Attempted to move null card to {newLocation}!");
                return;
            }

            if (card.CurrentLocation == newLocation)
            {
                return;
            }

            card.CurrentLocation = newLocation;
            GameEvents.RaiseCardLocationChanged(card);
        }

        /// <summary>
        /// Convenience function to handle cards moved by PlayerCharacter actions.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="action"></param>
        public void MoveCard(CardInstance card, PF.ActionType action)
        {
            if (card.Owner == null)
            {
                Debug.LogError($"[{GetType().Name}] {card.Data.cardName} has no owner - use MoveCard(CardInstance, CardLocation) instead!");
                return;
            }

            switch (action)
            {
                case PF.ActionType.Banish:
                    MoveCard(card, CardLocation.Vault);
                    // Don't clear out the owner yet - we might undo this.
                    break;
                case PF.ActionType.Bury:
                    MoveCard(card, CardLocation.Buried);
                    break;
                case PF.ActionType.Discard:
                    MoveCard(card, CardLocation.Discard);
                    break;
                case PF.ActionType.Display:
                    MoveCard(card, CardLocation.Displayed);
                    break;
                case PF.ActionType.Draw:
                    MoveCard(card, CardLocation.Hand);
                    break;
                case PF.ActionType.Recharge:
                    MoveCard(card, CardLocation.Deck);
                    card.Owner.Recharge(card);
                    break;
                case PF.ActionType.Reload:
                    MoveCard(card, CardLocation.Deck);
                    card.Owner.Reload(card);
                    break;
                case PF.ActionType.Reveal:
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

        private void HandleCardLocationChanged(CardInstance card)
        {
            if (card == null) return;

            theVault.Remove(card);
            if (card.CurrentLocation == CardLocation.Vault) theVault.Add(card);
        }

        // Find cards...
        public List<CardInstance> FindAll(System.Func<CardInstance, bool> predicate) => allCards.Where(predicate).ToList();
        // ... by location
        public List<CardInstance> GetCardsInLocation(CardLocation location) => FindAll(card => card.CurrentLocation == location);
        // ... owned by a specific player
        public List<CardInstance> GetCardsOwnedBy(PlayerCharacter owner) => FindAll(card => card.Owner == owner);
        // ... owned by a specific player in a specific location
        public List<CardInstance> GetCardsOwnedBy(PlayerCharacter owner, CardLocation location) => FindAll(card => card.Owner == owner && card.CurrentLocation == location);
        // ... are considered part of a specific player's hand (in hand and revealed)
        public List<CardInstance> GetCardsInHand(PlayerCharacter owner) => FindAll(card => card.Owner == owner && (card.CurrentLocation == CardLocation.Hand || card.CurrentLocation == CardLocation.Revealed));
    }
}
