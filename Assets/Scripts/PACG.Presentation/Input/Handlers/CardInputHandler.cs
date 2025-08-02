using PACG.Presentation.Cards;
using PACG.Presentation.UI.Controllers;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardInputHandler : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        var cardDisplay = GetComponent<CardDisplay>();
        ServiceLocator.Get<CardPreviewController>().ShowPreviewForCard(cardDisplay, null);
    }
}
