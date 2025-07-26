using UnityEngine;
using UnityEngine.UI;

public class CardPreviewController : MonoBehaviour
{
    public GameObject previewArea;
    public Button backgroundButton;

    private GameObject currentlyEnlargedCard;
    private Transform originalParent;
    private int originalSiblingIndex;
    private Vector3 originalScale;

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    private void Start()
    {
        // Add a listener to the background button to handle returning the card.
        backgroundButton.onClick.AddListener(ReturnCardToOrigin);
        previewArea.SetActive(false);
    }

    public void EnlargeCard(GameObject cardToEnlarge)
    {
        if (currentlyEnlargedCard != null) return;

        currentlyEnlargedCard = cardToEnlarge;

        // Store the card's original location and size.
        originalParent = cardToEnlarge.transform.parent;
        originalSiblingIndex = cardToEnlarge.transform.GetSiblingIndex();
        originalScale = cardToEnlarge.transform.localScale;

        // Show the preview area.
        previewArea.SetActive(true);

        // Move the card to the preview area and enlarge it.
        cardToEnlarge.transform.SetParent(previewArea.transform, true);
        cardToEnlarge.transform.localPosition = Vector3.zero;
        cardToEnlarge.transform.localScale = new Vector3(2f, 2f, 1.0f);
    }

    private void ReturnCardToOrigin()
    {
        if (currentlyEnlargedCard == null) return;

        // Return the card to its original parent and Z-index.
        currentlyEnlargedCard.transform.SetParent(originalParent, true);
        currentlyEnlargedCard.transform.SetSiblingIndex(originalSiblingIndex);
        currentlyEnlargedCard.transform.localScale = originalScale;

        // Hide the preview and clear the card.
        previewArea.SetActive(false);
        currentlyEnlargedCard = null;
    }
}
