using UnityEngine;

namespace PACG.Core
{
    public static class GuiUtils
    {
        public static Color32 GetPanelColor(PF.CardType cardType)
        {
            switch (cardType)
            {
                // Boons
                case PF.CardType.Ally:
                    return new Color32(68, 98, 153, 255);
                case PF.CardType.Armor:
                    return new Color32(170, 178, 186, 255);
                case PF.CardType.Blessing:
                    return new Color32(0, 172, 235, 255);
                case PF.CardType.Item:
                    return new Color32(96, 133, 132, 255);
                case PF.CardType.Spell:
                    return new Color32(97, 46, 138, 255);
                case PF.CardType.Weapon:
                    return new Color32(93, 97, 96, 255);

                // Banes
                case PF.CardType.Barrier:
                    return new Color32(255, 227, 57, 255);
                case PF.CardType.Monster:
                    return new Color32(213, 112, 41, 255);
                case PF.CardType.StoryBane:
                    return new Color32(130, 36, 38, 255);
                default:
                    Debug.LogError($"GetPanelColor --- Unknown card type: {cardType}");
                    return new Color32(255, 0, 255, 255);
            }
        }
    }
}
