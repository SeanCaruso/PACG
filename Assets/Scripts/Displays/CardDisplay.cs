using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class CardDisplay : MonoBehaviour
{
    [Header("System")]
    public TMP_FontAsset cardFont;

    [Header("Top/Bottom Bars")]
    public GameObject topPanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI typeText;
    public TextMeshProUGUI levelText;
    public GameObject bottomPanel;

    [Header("Check to Acquire/Defeat")]
    public GameObject checksLabelPanel;
    public TextMeshProUGUI checksLabelText;
    public GameObject checksSection;
    public GameObject checkDcPanel;
    public TextMeshProUGUI checkDC;
    public GameObject orPanel;
    public GameObject thenPanel;
    public GameObject checksSection2Area;
    public GameObject checksSection2;
    public TextMeshProUGUI checkDC2;

    [Header("Powers")]
    public GameObject powersPanel;
    public TextMeshProUGUI powersText;
    public GameObject recoveryLabel;
    public TextMeshProUGUI recoveryText;

    [Header("Traits")]
    public GameObject traitsSection;

    private CardData cardData;
    private GameContext gameContext;

    public void SetCardData(CardData cardData, GameContext context)
    {
        this.cardData = cardData;
        this.gameContext = context;
        UpdateCardDisplay();
    }

    public void UpdateCardDisplay()
    {
        if (cardData == null) return;

        // Set the various panel colors to the card type's color.
        Color panelColor = GetPanelColor(cardData.cardType);
        topPanel.GetComponent<Image>().color = panelColor;
        checksLabelPanel.GetComponent<Image>().color = Color.Lerp(panelColor, Color.black, 0.5f);
        checksSection.GetComponent<Image>().color = panelColor;
        checksSection2.GetComponent<Image>().color = panelColor;
        powersPanel.GetComponent<Image>().color = Color.Lerp(panelColor, Color.white, 0.5f);
        traitsSection.GetComponent<Image>().color = panelColor;

        // Top bar.
        nameText.text = cardData.name;
        typeText.text = PF.S(cardData.cardType);
        levelText.text = cardData.cardLevel.ToString();

        // Check to acquire/defeat.
        checksLabelText.text = PF.IsBoon(cardData.cardType) ? "CHECK TO ACQUIRE" : "CHECK TO DEFEAT";

        if (cardData.checkRequirement.mode == CheckMode.None)
        {
            // We need to resize the label to fit the whole section width (and get rid of the DC).
            var checksLabelRect = checksLabelPanel.GetComponent<RectTransform>();
            checksLabelRect.sizeDelta = new(80f, checksLabelRect.sizeDelta.y);
            checkDcPanel.SetActive(false);

            checksLabelText.text = "NONE";
        }
        else if (cardData.checkRequirement.checkSteps.Count > 0)
        {
            var check = cardData.checkRequirement.checkSteps[0];
            foreach (var skill in check.allowedSkills) AddTextToPanel(skill.ToString().ToUpper(), checksSection);

            int totalDC = check.baseDC + check.adventureLevelMult * gameContext.AdventureNumber;
            checkDC.text = totalDC.ToString();
        }

        // Add optional section for choice / sequential checks.
        if (cardData.checkRequirement.checkSteps.Count == 2)
        {
            if (cardData.checkRequirement.mode == CheckMode.Sequential)
            {
                thenPanel.SetActive(true);
            }
            else if (cardData.checkRequirement.mode == CheckMode.Choice)
            {
                orPanel.SetActive(true);
            }
            else
            {
                Debug.LogError($"UpdateCardDisplay --- {cardData.cardName} has multiple checks, but an invalid check mode!");
            }
            var check2 = cardData.checkRequirement.checkSteps[1];
            foreach (var skill in check2.allowedSkills) AddTextToPanel(skill.ToString().ToUpper(), checksSection2);

            int totalDC = check2.baseDC + check2.adventureLevelMult * gameContext.AdventureNumber;
            checkDC2.text = totalDC.ToString();
        }
        else if (cardData.checkRequirement.checkSteps.Count > 2)
        {
            Debug.LogError($"UpdateCardDisplay --- {cardData.cardName} has too many check steps!");
        }

        // Powers
        powersText.text = cardData.powers;
        if (cardData.recovery.Length > 0)
        {
            recoveryLabel.SetActive(true);
            recoveryText.enabled = true;
            recoveryText.text = cardData.recovery;
        }

        // Traits
        foreach (var trait in cardData.traits) AddTextToPanel(trait.ToString().ToUpper(), traitsSection);
    }

    private void AddTextToPanel(string text, GameObject panel)
    {
        GameObject textObject = new($"{text}_Object");
        textObject.transform.SetParent(panel.transform, false);

        TextMeshProUGUI tmp = textObject.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.font = cardFont;
        tmp.fontSize = 9f;
        tmp.color = Color.white;
    }

    private void Start()
    {
        UpdateCardDisplay();
    }

    private Color GetPanelColor(PF.CardType cardType)
    {
        switch(cardType)
        {
            // Boons
            case PF.CardType.Ally:
                return new(68, 98, 153);
            case PF.CardType.Armor:
                return new(170, 178, 186);
            case PF.CardType.Blessing:
                return new(0, 172, 235);
            case PF.CardType.Item:
                return new(96, 133, 132);
            case PF.CardType.Spell:
                return new(97, 46, 138);
            case PF.CardType.Weapon:
                return new(93, 97, 96);

            // Banes
            case PF.CardType.Barrier:
                return new(255, 227, 57);
            case PF.CardType.Monster:
                return new(213, 112, 41);
            case PF.CardType.StoryBane:
                return new(130, 36, 38);
            default:
                Debug.LogError($"GetPanelColor --- Unknown card type: {cardType}");
                return new(255, 0, 255);
        }
    }
}
