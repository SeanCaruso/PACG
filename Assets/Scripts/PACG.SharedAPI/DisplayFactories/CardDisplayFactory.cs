using PACG.Gameplay;
using PACG.Presentation;
using UnityEngine;

namespace PACG.SharedAPI
{
    public class CardDisplayFactory : MonoBehaviour
    {
        public enum DisplayContext
        {
            Default, // Default card view that can be clicked on to preview.
            Browser,
            Preview,
            GameStateIndicator
        }
        
        [Header("Prefabs")]
        public CardDisplay CardDisplayPrefab;

        public CardDisplay CreateCardDisplay(CardInstance card, DisplayContext context, Transform parent = null)
        {
            var display = Instantiate(CardDisplayPrefab, parent);
            display.SetViewModel(CardViewModelFactory.CreateFrom(card));
            ConfigureForContext(display, context);
            return display;
        }

        private static void ConfigureForContext(CardDisplay display, DisplayContext context)
        {
            // Reset transform to clean slate for layout system.
            display.transform.localPosition = Vector3.zero;
            display.transform.localScale = Vector3.one;
            display.transform.localRotation = Quaternion.identity;
            
            // Add behavioral components.
            switch (context)
            {
                case DisplayContext.Default:
                    display.gameObject.AddComponent<CardInputHandler>();
                    break;
                case DisplayContext.Browser:
                    display.gameObject.AddComponent<CardInputHandler>();
                    display.gameObject.AddComponent<CardDragHandler>();
                    break;
                case DisplayContext.Preview:
                    // No components - preview manages its own interactions.
                    break;
                case DisplayContext.GameStateIndicator:
                    if (!display.gameObject.TryGetComponent<CanvasGroup>(out var canvasGroup))
                        canvasGroup = display.gameObject.AddComponent<CanvasGroup>();
                    canvasGroup.blocksRaycasts = false;
                    break;
            }
        }
    }
}
