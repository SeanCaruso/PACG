using PACG.Gameplay;
using PACG.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace PACG.SharedAPI
{
    public class DiscardViewController : MonoBehaviour
    {
        [Header("Dependencies")]
        public CardDisplayFactory CardDisplayFactory;
        public DeckExamineController DeckExamineController;

        [Header("UI Components")]
        public Button Button;
        public Transform CardDisplayContainer;

        private CardDisplay _currentCardDisplay;
        private PlayerCharacter _currentPC;

        private void OnEnable()
        {
            Button.onClick.AddListener(OnClicked);
            
            GameEvents.PlayerCharacterChanged += OnPlayerCharacterChanged;
            GameEvents.CardLocationChanged += OnCardLocationChanged;
        }
        
        private void OnDisable()
        {
            Button.onClick.RemoveListener(OnClicked);
            
            GameEvents.PlayerCharacterChanged -= OnPlayerCharacterChanged;
            GameEvents.CardLocationChanged -= OnCardLocationChanged;
        }

        private void OnClicked()
        {
            Debug.Log("Discards Clicked");
        }
        
        private void OnPlayerCharacterChanged(PlayerCharacter pc)
        {
            _currentPC = pc;
        }

        private void OnCardLocationChanged(CardInstance card)
        {
            if (card.Owner != _currentPC || card.CurrentLocation != CardLocation.Discard) return;
            
            if (_currentCardDisplay)
                Destroy(_currentCardDisplay.gameObject);

            _currentCardDisplay = CardDisplayFactory.CreateCardDisplay(
                card,
                CardDisplayFactory.DisplayContext.GameStateIndicator,
                CardDisplayContainer
            );
        }
    }
}
