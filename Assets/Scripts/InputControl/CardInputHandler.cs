using UnityEngine;
using UnityEngine.EventSystems;

public class CardInputHandler : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        ServiceLocator.Get<CardPreviewController>().EnlargeCard(this.gameObject);
    }
}
