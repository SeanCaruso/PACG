using PACG.Gameplay;
using System.Linq;
using UnityEngine;

namespace PACG.SharedAPI
{
    public static class CardViewModelFactory
    {
        public static CardViewModel CreateFrom(CardInstance card)
        {
            if (card == null) return null;

            var data = card.Data;

            var viewModel = new CardViewModel(card)
            {
                PanelColor = GetPanelColor(data.cardType)
            };

            if (data.checkRequirement.mode == CheckMode.None)
            {
                if (data.checkRequirement.checkSteps.Count > 0)
                    Debug.LogWarning($"{data.name} has CheckMode.None with check steps!");

                viewModel.Check1Skills = new[] { "NONE" };
                // We're done!
                return viewModel;
            }

            if (data.checkRequirement.checkSteps.Count == 0)
            {
                Debug.LogError($"{data.name} has CheckMode {data.checkRequirement.mode} but no check steps! Aborting!");
                return null;
            }

            var checkStep1 = data.checkRequirement.checkSteps[0];
            viewModel.Check1Skills = checkStep1.category == CheckCategory.Combat
                ? new[] { "COMBAT" }
                : checkStep1.allowedSkills.Select(s => s.ToString().ToUpper());
            viewModel.Check1Dc = CardUtils.GetDc(checkStep1.baseDC, checkStep1.adventureLevelMult).ToString();
            
            if (data.checkRequirement.mode == CheckMode.Single)
            {
                if (data.checkRequirement.checkSteps.Count > 1)
                    Debug.LogWarning($"{data.name} has CheckMode.Single but more than one check step!");
                return viewModel;
            }

            if (data.checkRequirement.checkSteps.Count != 2)
            {
                Debug.LogError($"{data.name} has CheckMode {data.checkRequirement.mode} but {data.checkRequirement.checkSteps.Count} check steps! Aborting!");
                return null;
            }

            var checkStep2 = data.checkRequirement.checkSteps[1];
            viewModel.Check2Skills = checkStep2.category == CheckCategory.Combat
                ? new[] { "COMBAT" }
                : checkStep2.allowedSkills.Select(s => s.ToString().ToUpper());
            viewModel.Check2Dc = CardUtils.GetDc(checkStep2.baseDC, checkStep2.adventureLevelMult).ToString();

            return viewModel;
        }

        private static Color32 GetPanelColor(PF.CardType cardType)
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
