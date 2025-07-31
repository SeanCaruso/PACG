using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    // --- Events ---
    public event Action<CardInstance> OnCardLocationChanged;
    // --------------

    private readonly List<CardInstance> allCards = new();
    private readonly List<CardInstance> theVault = new();

    private void Awake()
    {
        ServiceLocator.Register(this);
        OnCardLocationChanged += HandleCardLocationChanged;
    }

    private void OnDestroy()
    {
        OnCardLocationChanged -= HandleCardLocationChanged;
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

    public void MoveCard(CardInstance card, CardLocation newLocation)
    {
        if (card == null)
        {
            Debug.LogError($"Attempted to move null card to {newLocation}!");
            return;
        }

        card.CurrentLocation = newLocation;
        OnCardLocationChanged?.Invoke(card);

        Debug.Log($"Moved {card.Data.cardName} to {newLocation}");
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
