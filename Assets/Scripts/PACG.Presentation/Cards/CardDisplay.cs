using PACG.SharedAPI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PACG.Presentation
{
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
        public GameObject StoryBaneTypePanel;
        public TextMeshProUGUI StoryBaneTypeText;
        public GameObject LootPanel;
        public GameObject traitsSection;

        public CardViewModel ViewModel { get; private set; }
        public void SetViewModel(CardViewModel view)
        {
            if (view == null) return;
            
            ViewModel = view;

            // Set the various panel colors to the card type's color.
            topPanel.GetComponent<Image>().color = view.PanelColor;
            checksLabelPanel.GetComponent<Image>().color = Color.Lerp(view.PanelColor, Color.black, 0.75f);
            checksSection.GetComponent<Image>().color = view.PanelColor;
            checksSection2.GetComponent<Image>().color = view.PanelColor;
            powersPanel.GetComponent<Image>().color = Color.Lerp(view.PanelColor, Color.white, 0.75f);
            traitsSection.GetComponent<Image>().color = view.PanelColor;
            bottomPanel.GetComponent<Image>().color = view.PanelColor;

            // Top bar.
            nameText.text = view.Name;
            typeText.text = view.Type;
            levelText.text = view.Level;

            // Check to acquire/defeat.
            UpdateChecksSection(view);

            // Powers
            powersText.text = view.PowersText;
            if (view.RecoveryText.Length > 0)
            {
                recoveryLabel.SetActive(true);
                recoveryText.enabled = true;
                recoveryText.text = view.RecoveryText;
            }

            // Traits
            StoryBaneTypePanel.SetActive(view.IsStoryBane);
            StoryBaneTypePanel.GetComponent<Image>().color = view.StoryBaneTypePanelColor;
            StoryBaneTypeText.text = view.StoryBaneType;
            LootPanel.SetActive(view.IsLoot);
            foreach (var trait in view.Traits) AddTextToPanel(trait, traitsSection);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(traitsSection.GetComponent<RectTransform>());
        }

        private void UpdateChecksSection(CardViewModel view)
        {
            checksLabelText.text = view.ChecksLabel;
            foreach (var skill in view.Check1Skills) AddTextToPanel(skill, checksSection, 8f);

            if (view.CheckMode == CheckMode.None)
            {
                // We need to resize the label to fit the whole section width (and get rid of the DC).
                var checksSectionRect = checksSection.GetComponent<RectTransform>();
                checksSectionRect.sizeDelta = new Vector2(80f, checksSectionRect.sizeDelta.y);
                checkDcPanel.SetActive(false);
                return;
            }

            checkDC.text = view.Check1Dc;

            // If we don't have Check2, return.
            if (!view.ShowCheck2) return;
            
            checksSection2Area.SetActive(true);

            switch (view.CheckMode)
            {
                // Set the THEN or OR panel active based on the check mode.
                case CheckMode.Sequential:
                    thenPanel.SetActive(true);
                    break;
                case CheckMode.Choice:
                    orPanel.SetActive(true);
                    break;
                default:
                    Debug.LogError($"UpdateCardDisplay --- {view.Name} has multiple checks, but an invalid check mode!");
                    break;
            }

            foreach (var skill in view.Check2Skills) AddTextToPanel(skill, checksSection2, 8f);
            checkDC2.text = view.Check2Dc;

        }

        private void AddTextToPanel(string text, GameObject panel, float fontSize = 9f)
        {
            GameObject textObject = new($"{text}_Object");
            textObject.transform.SetParent(panel.transform, false);

            var tmp = textObject.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.font = cardFont;
            tmp.fontSize = fontSize;
            tmp.color = Color.white;

            var sizeFitter = textObject.AddComponent<ContentSizeFitter>();
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }
}
