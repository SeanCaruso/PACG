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
        if (cardAction == null || cardAction?.Card == null) return;

        // Set card art if available.
        if (cardArtImage && cardAction.Card.Data && cardAction.Card.Data.cardArt)
            cardArtImage.sprite = cardAction.Card.Data.cardArt;

        // Set card name.
        if (cardNameText && cardAction.Card.Data)
            cardNameText.text = cardAction.Card.Data.cardName;

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
