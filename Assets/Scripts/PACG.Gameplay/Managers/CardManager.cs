

using PACG.SharedAPI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PACG.Gameplay
{
    public class CardManager : MonoBehaviour
    {
        private readonly List<CardInstance> allCards = new();
        private readonly List<CardInstance> theVault = new();

        private readonly Dictionary<CardInstance, CardLocation> stagedOriginalLocations = new();

        private void Awake()
        {
            ServiceLocator.Register(this);
            GameEvents.CardLocationChanged += HandleCardLocationChanged;
        }

        private void OnDestroy()
        {
            GameEvents.CardLocationChanged -= HandleCardLocationChanged;
        }

        public CardInstance New(CardData card, PlayerCharacter owner = null)
        {
            if (card == null)
            {
                Debug.LogError("Cannot create a card from null CardData!");
                return null;
            }

            CardInstance newInstance = new(card, owner);
            allCards.Add(newInstance);

            return newInstance;
        }

        public void MoveCard(CardInstance card, CardLocation newLocation, bool skipStage = false)
        {
            if (card == null)
            {
                Debug.LogError($"Attempted to move null card to {newLocation}!");
                return;
            }

            if (!skipStage)
            {
                stagedOriginalLocations.TryAdd(card, card.CurrentLocation);
            }

            card.CurrentLocation = newLocation;
            GameEvents.RaiseCardLocationChanged(card);

            Debug.Log($"Moved {card.Data.cardName} to {newLocation}");
        }

        public void RestoreStagedCards()
        {
            foreach ((var card, var location) in stagedOriginalLocations)
            {
                MoveCard(card, location, true);
            }
            stagedOriginalLocations.Clear();
        }

        public void CommitStagedMoves()
        {
            stagedOriginalLocations.Clear();
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
        // ... in a specific player's hand
        public List<CardInstance> GetCardsInPlayerHand(PlayerCharacter player) => FindAll(card => card.Owner == player && card.CurrentLocation == CardLocation.Hand);
    }
}
