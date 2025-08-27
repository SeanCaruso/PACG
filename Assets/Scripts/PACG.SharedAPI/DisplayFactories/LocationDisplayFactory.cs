using PACG.Gameplay;
using PACG.Presentation;
using UnityEngine;

namespace PACG.SharedAPI
{
    public class LocationDisplayFactory : MonoBehaviour
    {
        public enum DisplayContext
        {
            Default, // Default card view that can be clicked on to preview.
            Browser,
            Preview,
            GameStateIndicator
        }
        
        [Header("Prefabs")]
        public LocationDisplay LocationDisplayPrefab;

        public LocationDisplay CreateLocationDisplay(Location location, DisplayContext context, Transform parent = null)
        {
            var display = Instantiate(LocationDisplayPrefab, parent);
            display.SetViewModel(LocationViewModelFactory.CreateFrom(location));
            ConfigureForContext(display, context);
            return display;
        }

        private static void ConfigureForContext(LocationDisplay display, DisplayContext context)
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
