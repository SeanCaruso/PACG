using System.Collections.Generic;
using System.Linq;
using PACG.Gameplay;
using PACG.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace PACG.SharedAPI
{
    public class HandViewController : MonoBehaviour
    {
        [Header("Dependencies")]
        public CardDisplayFactory CardDisplayFactory;

        [Header("UI Elements")]
        public Transform HandContainer;

        private const float MaxWidth = 1045f;
        
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
            
            for (var i = HandContainer.childCount - 1; i >= 0; i--)
                Destroy(HandContainer.GetChild(i).gameObject);
            _cardInstancesToDisplaysMap.Clear();
            
            foreach (var card in _currentPC.Hand)
                CreateDisplayForCard(card);
            
            AdjustSpacing();
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
            
            AdjustSpacing();
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

        private void AdjustSpacing()
        {
            var layout = HandContainer.gameObject.GetComponent<HorizontalLayoutGroup>();
            if (_cardInstancesToDisplaysMap.Count <= 4)
            {
                layout.spacing = 15f;
                return;
            }
            
            var cardWidth = _cardInstancesToDisplaysMap.Values.First().GetComponent<RectTransform>().rect.width;
            var totalCardsWidth = _cardInstancesToDisplaysMap.Count * cardWidth;
            var extraSpace = totalCardsWidth - MaxWidth;
            layout.spacing = -extraSpace / (_cardInstancesToDisplaysMap.Count - 1);
        }
    }
}
