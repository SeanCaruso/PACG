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

    [Header("States")]
    public bool isInHand = false;
    public bool isExpanded = false;
    public bool isStaged = false;

    public Vector3 originalPosition;
    public Vector3 originalScale;
    public int originalSiblingIndex;

    private CardInstance cardInstance;
    public IEncounterLogic encounterLogic = null;
    public IPlayableLogic playableLogic = null;

    public void SetCardInstance(CardInstance card)
    {
        this.cardInstance = card;
        UpdateCardDisplay();

        var logicRegistry = ServiceLocator.Get<LogicRegistry>();
        encounterLogic = logicRegistry.GetEncounterLogic(card);
        playableLogic = logicRegistry.GetPlayableLogic(card);
    }

    public CardInstance Card => cardInstance;

    public void UpdateCardDisplay()
    {
        if (cardInstance == null) return;

        // Set the various panel colors to the card type's color.
        Color32 panelColor = GetPanelColor(cardInstance.Data.cardType);
        topPanel.GetComponent<Image>().color = panelColor;
        checksLabelPanel.GetComponent<Image>().color = Color.Lerp(panelColor, Color.black, 0.75f);
        checksSection.GetComponent<Image>().color = panelColor;
        checksSection2.GetComponent<Image>().color = panelColor;
        powersPanel.GetComponent<Image>().color = Color.Lerp(panelColor, Color.white, 0.75f);
        traitsSection.GetComponent<Image>().color = panelColor;
        bottomPanel.GetComponent<Image>().color = panelColor;

        // Top bar.
        nameText.text = cardInstance.Data.name;
        typeText.text = PF.S(cardInstance.Data.cardType);
        levelText.text = cardInstance.Data.cardLevel.ToString();

        // Check to acquire/defeat.
        checksLabelText.text = PF.IsBoon(cardInstance.Data.cardType) ? "CHECK TO ACQUIRE" : "CHECK TO DEFEAT";

        if (cardInstance.Data.checkRequirement.mode == CheckMode.None)
        {
            // We need to resize the label to fit the whole section width (and get rid of the DC).
            var checksLabelRect = checksLabelPanel.GetComponent<RectTransform>();
            checksLabelRect.sizeDelta = new(80f, checksLabelRect.sizeDelta.y);
            checkDcPanel.SetActive(false);

            checksLabelText.text = "NONE";
        }
        else if (cardInstance.Data.checkRequirement.checkSteps.Count > 0)
        {
            var check = cardInstance.Data.checkRequirement.checkSteps[0];
            if (check.category == CheckCategory.Combat)
            {
                AddTextToPanel("COMBAT", checksSection, 8f);
            }
            else
            {
                foreach (var skill in check.allowedSkills) AddTextToPanel(skill.ToString().ToUpper(), checksSection, 8f);
            }

            int totalDC = check.baseDC + check.adventureLevelMult * Game.GameContext.AdventureNumber;
            checkDC.text = totalDC.ToString();
        }

        // Add optional section for choice / sequential checks.
        if (cardInstance.Data.checkRequirement.checkSteps.Count == 2)
        {
            if (cardInstance.Data.checkRequirement.mode == CheckMode.Sequential)
            {
                thenPanel.SetActive(true);
            }
            else if (cardInstance.Data.checkRequirement.mode == CheckMode.Choice)
            {
                orPanel.SetActive(true);
            }
            else
            {
                Debug.LogError($"UpdateCardDisplay --- {cardInstance.Data.cardName} has multiple checks, but an invalid check mode!");
            }
            var check2 = cardInstance.Data.checkRequirement.checkSteps[1];
            if (check2.category == CheckCategory.Combat)
            {
                AddTextToPanel("COMBAT", checksSection2, 8f);
            }
            else
            {
                foreach (var skill in check2.allowedSkills) AddTextToPanel(skill.ToString().ToUpper(), checksSection2, 8f);
            }

            int totalDC = check2.baseDC + check2.adventureLevelMult * Game.GameContext.AdventureNumber;
            checkDC2.text = totalDC.ToString();
        }
        else if (cardInstance.Data.checkRequirement.checkSteps.Count > 2)
        {
            Debug.LogError($"UpdateCardDisplay --- {cardInstance.Data.cardName} has too many check steps!");
        }

        // Powers
        powersText.text = cardInstance.Data.powers;
        if (cardInstance.Data.recovery.Length > 0)
        {
            recoveryLabel.SetActive(true);
            recoveryText.enabled = true;
            recoveryText.text = cardInstance.Data.recovery;
        }

        // Traits
        foreach (var trait in cardInstance.Data.traits) AddTextToPanel(trait.ToString().ToUpper(), traitsSection);
    }

    private void AddTextToPanel(string text, GameObject panel, float fontSize = 9f)
    {
        GameObject textObject = new($"{text}_Object");
        textObject.transform.SetParent(panel.transform, false);

        TextMeshProUGUI tmp = textObject.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.font = cardFont;
        tmp.fontSize = fontSize;
        tmp.color = Color.white;

        var sizeFitter = textObject.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private void Start()
    {
        //UpdateCardDisplay();
    }

    private Color32 GetPanelColor(PF.CardType cardType)
    {
        switch(cardType)
        {
            // Boons
            case PF.CardType.Ally:
                return new(68, 98, 153, 255);
            case PF.CardType.Armor:
                return new(170, 178, 186, 255);
            case PF.CardType.Blessing:
                return new(0, 172, 235, 255);
            case PF.CardType.Item:
                return new(96, 133, 132, 255);
            case PF.CardType.Spell:
                return new(97, 46, 138, 255);
            case PF.CardType.Weapon:
                return new(93, 97, 96, 255);

            // Banes
            case PF.CardType.Barrier:
                return new(255, 227, 57, 255);
            case PF.CardType.Monster:
                return new(213, 112, 41, 255);
            case PF.CardType.StoryBane:
                return new(130, 36, 38, 255);
            default:
                Debug.LogError($"GetPanelColor --- Unknown card type: {cardType}");
                return new(255, 0, 255, 255);
        }
    }
}
