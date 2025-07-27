using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardActionButton : MonoBehaviour
{
    [Header("Visual Elements")]
    public Image cardArtImage;
    public TextMeshProUGUI actionTypeText;
    public TextMeshProUGUI cardNameText;
    public Image actionTypeIcon;

    private PlayCardAction cardAction;

    public void Setup(PlayCardAction action)
    {
        cardAction = action;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (cardAction == null || cardAction?.CardData == null) return;

        // Set card art if available.
        if (cardArtImage && cardAction.CardData && cardAction.CardData.cardArt)
            cardArtImage.sprite = cardAction.CardData.cardArt;

        // Set card name.
        if (cardNameText && cardAction.CardData)
            cardNameText.text = cardAction.CardData.cardName;

        // Set action type.
        if (actionTypeText)
            actionTypeText.text = cardAction.GetLabel();

        // Optional: Set action type icon based on action type.
        if (actionTypeIcon)
        {
            // Not implemented.
        }
    }
}
