using UnityEngine;
using UnityEngine.UI;

public class CanvasSetup : MonoBehaviour
{
    [Header("UI Containers")]
    // public RectTransform backgroundContainer;
    public RectTransform gameAreasContainer;
    public RectTransform cardsContainer;
    public RectTransform popupContainer;

    private void Start()
    {
        // Ensure correct rendering order.
        gameAreasContainer.SetAsFirstSibling();
        cardsContainer.SetSiblingIndex(1);
        popupContainer.SetAsLastSibling();
    }

    public void BringToFront(Transform element)
    {
        element.SetAsLastSibling();
    }
}
