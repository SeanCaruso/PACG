using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public CardData cardData;

    public TextMeshProUGUI nameText;
    public Image artImage;

    public void SetCardData(CardData cardData)
    {
        this.cardData = cardData;
        UpdateCardDisplay();
    }

    public void UpdateCardDisplay()
    {
        if (cardData != null)
        {
            nameText.text = cardData.cardName;
            artImage.sprite = cardData.cardArt;
        }
    }

    private void Start()
    {
        UpdateCardDisplay();
    }
}
