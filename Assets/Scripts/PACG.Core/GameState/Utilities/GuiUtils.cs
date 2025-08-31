using PACG.Data;
using UnityEngine;

namespace PACG.Core
{
    public static class GuiUtils
    {
        public static Color32 GetPanelColor(CardType cardType)
        {
            switch (cardType)
            {
                // Boons
                case CardType.Ally:
                    return new Color32(68, 98, 153, 255);
                case CardType.Armor:
                    return new Color32(170, 178, 186, 255);
                case CardType.Blessing:
                    return new Color32(0, 172, 235, 255);
                case CardType.Item:
                    return new Color32(96, 133, 132, 255);
                case CardType.Spell:
                    return new Color32(97, 46, 138, 255);
                case CardType.Weapon:
                    return new Color32(93, 97, 96, 255);

                // Banes
                case CardType.Barrier:
                    return new Color32(255, 227, 57, 255);
                case CardType.Monster:
                    return new Color32(213, 112, 41, 255);
                case CardType.StoryBane:
                    return new Color32(130, 36, 38, 255);
                
                // Other
                case CardType.Location:
                    return new Color32(135, 113, 84, 255);
                default:
                    Debug.LogError($"GetPanelColor --- Unknown card type: {cardType}");
                    return new Color32(255, 0, 255, 255);
            }
        }
    }
}
