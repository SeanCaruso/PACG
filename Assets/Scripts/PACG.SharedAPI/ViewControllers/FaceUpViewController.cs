using System.Collections.Generic;
using PACG.Gameplay;
using PACG.Presentation;
using UnityEngine;

namespace PACG.SharedAPI
{
    public class FaceUpViewController : MonoBehaviour
    {
        [Header("Dependencies")]
        public CardDisplayFactory CardDisplayFactory;

        [Header("UI Elements")]
        public Transform DisplayedContainer;
        public Transform RecoveryContainer;
        public Transform RevealedContainer;
        
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
            
            // If we're moving from a face-up location, remove the existing display.
            if (_cardInstancesToDisplaysMap.TryGetValue(card, out var cardDisplay))
            {
                Destroy(cardDisplay.gameObject);
                _cardInstancesToDisplaysMap.Remove(card);
            }
            
            // If we're moving to a face-up location, create a new display.
            var targetParent = card.CurrentLocation switch
            {
                CardLocation.Displayed => DisplayedContainer,
                CardLocation.Recovery => RecoveryContainer,
                CardLocation.Revealed => RevealedContainer,
                _ => null
            };

            if (!targetParent) return;

            var newCardDisplay = CardDisplayFactory.CreateCardDisplay(
                card,
                CardDisplayFactory.DisplayContext.Default,
                targetParent
            );
            _cardInstancesToDisplaysMap[card] = newCardDisplay;
        }
    }
}
