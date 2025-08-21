using PACG.Gameplay;
using TMPro;
using UnityEngine;

namespace PACG.Presentation.SkillSelectionDialog
{
    public class SkillSelectionDialog : MonoBehaviour
    {
        [Header("UI Elements")]
        public TextMeshProUGUI LabelText; // Check to Defeat/Acquire
        public TextMeshProUGUI CardNameText;
        public Transform DropdownContainer;
        
        [Header("Prefabs")]
        public SkillDropdownPanel SkillDropdownPanel;
        
        public void SetCheckContext(CheckContext context)
        {
            if (context.Resolvable?.Card is not CardInstance card) return;
            
            LabelText.text = PF.IsBane(card.Data.cardType) ? "CHECK TO DEFEAT" : "CHECK TO ACQUIRE";
            CardNameText.text = context.Resolvable.Card.Name.ToUpper();
            
            var dropdown = Instantiate(SkillDropdownPanel, DropdownContainer);
            dropdown.SetCheckContext(context);
        }
    }
}
