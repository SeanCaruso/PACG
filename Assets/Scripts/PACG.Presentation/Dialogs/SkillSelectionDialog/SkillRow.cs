using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PACG.Presentation.SkillSelectionDialog
{
    public class SkillRow : MonoBehaviour
    {
        [Header("UI Elements")]
        public Image DieImage;
        public TextMeshProUGUI DieText;
        public TextMeshProUGUI SkillBonusText;
        public TextMeshProUGUI SkillNameText;
        public TextMeshProUGUI DcText;

        [Header("Dice Sprites")]
        public Sprite D4Sprite;
        public Sprite D6Sprite;
        public Sprite D8Sprite;
        public Sprite D10Sprite;
        public Sprite D12Sprite;
        public Sprite D20Sprite;

        public Sprite GetDieSprite(int sides)
        {
            return sides switch
            {
                4 => D4Sprite,
                6 => D6Sprite,
                8 => D8Sprite,
                10 => D10Sprite,
                12 => D12Sprite,
                20 => D20Sprite,
                _ => null
            };
        }
    }
}
