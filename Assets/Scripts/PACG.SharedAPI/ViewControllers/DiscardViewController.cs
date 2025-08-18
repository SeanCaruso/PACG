using System.Collections.Generic;
using System.Linq;
using PACG.Gameplay;
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
        public Transform DiscardsContainer;

        private readonly List<CardInstance> _discards = new();
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
            if (_currentPC == null || _currentPC.Discards.Count == 0) return;
            
            if (DiscardsContainer.childCount != 1)
            {
                Debug.LogError($"[TurnCardDisplayController{GetType().Name}] Discard pile child count is not 1!");
                return;
            }

            var context = new ExamineContext
            {
                ExamineMode = ExamineContext.Mode.Discard,
                Cards = _currentPC.Discards.ToList(),
                UnknownCount = 0,
                CanReorder = false,
                OnClose = () => { }
            };
            
            DeckExamineController.OnExamineEvent(context);
        }
        
        private void OnPlayerCharacterChanged(PlayerCharacter pc)
        {
            _currentPC = pc;
        }

        private void OnCardLocationChanged(CardInstance card)
        {
            if (card.Owner != _currentPC) return;
            
            // Card entering discard.
            if (!_discards.Contains(card) && card.CurrentLocation == CardLocation.Discard)
            {
                _discards.Add(card);
                UpdateShownDiscard();
            }
            // Card leaving discard.
            else if (_discards.Contains(card) && card.CurrentLocation != CardLocation.Discard)
            {
                _discards.Remove(card);
                UpdateShownDiscard();
            }
        }

        private void UpdateShownDiscard()
        {
            // There should only be one discard, but loop through just in case.
            for (var i = DiscardsContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(DiscardsContainer.GetChild(i).gameObject);
            }
            
            if (_discards.Count == 0) return;
            
            CardDisplayFactory.CreateCardDisplay(
                _discards[^1],
                CardDisplayFactory.DisplayContext.GameStateIndicator,
                DiscardsContainer
            );
        }
    }
}
