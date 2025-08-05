using UnityEngine;
using UnityEngine.EventSystems;

namespace PACG.Presentation
{
    public class CardInputHandler : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            var cardDisplay = GetComponent<CardDisplay>();
            ServiceLocator.Get<CardPreviewController>().ShowPreviewForCard(cardDisplay);
        }
    }
}
