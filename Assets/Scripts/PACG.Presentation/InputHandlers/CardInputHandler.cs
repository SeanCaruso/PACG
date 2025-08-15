using PACG.SharedAPI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PACG.Presentation
{
    public class CardInputHandler : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            var cardDisplay = GetComponent<CardDisplay>();
            var previewController = FindFirstObjectByType<CardPreviewController>();
            if (previewController == null)
            {
                Debug.LogError("Unable to find CardPreviewController - does it exist in the scene?");
                return;
            }
            previewController.ShowPreviewForCard(cardDisplay);
        }
    }
}
