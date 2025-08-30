using PACG.Core;
using PACG.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PACG.Presentation.SkillSelectionDialog
{
    public class SkillSelectionDialog : MonoBehaviour
    {
        [Header("UI Elements")]
        public TextMeshProUGUI LabelText; // Check to Defeat/Acquire
        public Image CardNamePanel;
        public TextMeshProUGUI CardNameText;
        public Transform DropdownContainer;
        
        [Header("Prefabs")]
        public SkillDropdownPanel SkillDropdownPanel;
        
        public void SetCheckContext(CheckContext context)
        {
            if (context.Resolvable == null) return;

            LabelText.text = $"CHECK TO {context.Resolvable.Verb.ToString().ToUpper()}";
            CardNameText.text = context.Resolvable.Card.Name.ToUpper();
            CardNamePanel.color = GuiUtils.GetPanelColor(context.Resolvable.Card.CardType);
            
            var dropdown = Instantiate(SkillDropdownPanel, DropdownContainer);
            dropdown.SetCheckContext(context, context.Resolvable.Card.CardType);
        }
    }
}
