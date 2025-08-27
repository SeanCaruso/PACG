using PACG.SharedAPI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PACG.Presentation
{
    public class LocationDisplay : MonoBehaviour
    {
        [Header("System")]
        public TMP_FontAsset CardFont;

        [Header("Top/Bottom Bars")]
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI LevelText;

        [Header("Powers")]
        public TextMeshProUGUI AtLocationText;
        public Image AtLocationImage;
        public Button AtLocationButton;
        public TextMeshProUGUI ToCloseText;
        public Image ToCloseImage;
        public Button ToCloseButton;
        public TextMeshProUGUI WhenClosedText;
        public Image WhenClosedImage;
        public Button WhenClosedButton;

        [Header("Traits")]
        public GameObject TraitsSection;
        
        public LocationViewModel ViewModel { get; private set; }
        public void SetViewModel(LocationViewModel view)
        {
            if (view == null) return;
            
            ViewModel = view;
            
            // Top bar.
            NameText.text = view.Name;
            LevelText.text = view.Level;
            
            // Power text
            AtLocationText.text = view.AtLocationText;
            ToCloseText.text = view.ToCloseText;
            WhenClosedText.text = view.WhenClosedText;
            
            // Activated powers
            AtLocationButton.gameObject.SetActive(view.HasAtLocationPower);
            AtLocationButton.enabled = false;
            AtLocationImage.sprite = view.AtLocationDisabledSprite;
            
            ToCloseButton.gameObject.SetActive(view.HasToClosePower);
            ToCloseButton.enabled = false;
            ToCloseImage.sprite = view.ToCloseDisabledSprite;
            
            WhenClosedButton.gameObject.SetActive(view.HasWhenClosedPower);
            WhenClosedButton.enabled = false;
            WhenClosedImage.sprite = view.WhenClosedDisabledSprite;
            
            // Clear the traits section.
            foreach (Transform child in TraitsSection.transform)
                Destroy(child.gameObject);
            
            // Traits
            foreach (var trait in view.Traits)
            {
                GameObject textObject = new($"{trait}_Object");
                textObject.transform.SetParent(TraitsSection.transform, false);

                var tmp = textObject.AddComponent<TextMeshProUGUI>();
                tmp.text = trait;
                tmp.font = CardFont;
                tmp.fontSize = 11f;
                tmp.color = Color.white;

                var sizeFitter = textObject.AddComponent<ContentSizeFitter>();
                sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
        }

        public void SetAtLocationPowerEnabled(bool isEnabled)
        {
            AtLocationButton.enabled = isEnabled;
            AtLocationImage.sprite = isEnabled 
                ? ViewModel.AtLocationEnabledSprite
                : ViewModel.AtLocationDisabledSprite;
        }

        public void SetToClosePowerEnabled(bool isEnabled)
        {
            ToCloseButton.enabled = isEnabled;
            ToCloseImage.sprite = isEnabled 
                ? ViewModel.ToCloseEnabledSprite
                : ViewModel.ToCloseDisabledSprite;
        }

        public void SetWhenClosedPowerEnabled(bool isEnabled)
        {
            WhenClosedButton.enabled = isEnabled;
            WhenClosedImage.sprite = isEnabled 
                ? ViewModel.WhenClosedEnabledSprite
                : ViewModel.WhenClosedDisabledSprite;
        }
    }
}
