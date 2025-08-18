using System.Collections.Generic;
using PACG.Gameplay;
using PACG.Presentation;
using UnityEngine;

namespace PACG.SharedAPI
{
    public class HandViewController : MonoBehaviour
    {
        [Header("Dependencies")]
        public CardDisplayFactory CardDisplayFactory;

        [Header("UI Elements")]
        public Transform HandContainer;
        
        private PlayerCharacter _currentPC;
        private readonly Dictionary<CardInstance, CardDisplay> _cardInstancesToDisplaysMap = new();

        private void OnEnable()
        {
            GameEvents.PlayerCharacterChanged += OnPlayerCharacterChanged;
            GameEvents.CardLocationChanged += OnCardLocationChanged;
        }
        
        private void OnDisable()
        {
            GameEvents.PlayerCharacterChanged -= OnPlayerCharacterChanged;
            GameEvents.CardLocationChanged -= OnCardLocationChanged;
        }

        private void OnPlayerCharacterChanged(PlayerCharacter pc)
        {
            _currentPC = pc;
        }
        
        private void OnCardLocationChanged(CardInstance card)
        {
            if (card.Owner != _currentPC) return;
            
            // Card entering hand.
            if (card.CurrentLocation == CardLocation.Hand)
            {
                CreateDisplayForCard(card);
            }
            // Card leaving hand.
            else if (_cardInstancesToDisplaysMap.ContainsKey(card))
            {
                RemoveDisplayForCard(card);
            }
        }

        private void CreateDisplayForCard(CardInstance card)
        {
            var cardDisplay = CardDisplayFactory.CreateCardDisplay(
                card,
                CardDisplayFactory.DisplayContext.Default,
                HandContainer
            );
            _cardInstancesToDisplaysMap[card] = cardDisplay;
        }
        
        private void RemoveDisplayForCard(CardInstance card)
        {
            if (!_cardInstancesToDisplaysMap.TryGetValue(card, out var cardDisplay)) return;
            
            Destroy(cardDisplay.gameObject);
            _cardInstancesToDisplaysMap.Remove(card);
        }
    }
}
