using PACG.SharedAPI.ViewModels;
using System.Linq;
using UnityEngine;

namespace PACG.Services.ViewModelFactories
{
    public static class CardViewModelFactory
    {
        public static CardViewModel CreateFrom(CardInstance card, int adventureNumber)
        {
            if (card == null) return null;

            var data = card.Data;

            var viewModel = new CardViewModel
            {
                Name = data.cardName.ToUpper(),
                Type = data.cardType.ToString().ToUpper(),
                Level = data.cardLevel.ToString().ToUpper(),
                // TODO: Replace # in power text with adventure level calculation.
                PowersText = data.powers,
                RecoveryText = data.recovery,

                PanelColor = GetPanelColor(data.cardType),
                Traits = data.traits.Select(t => t.ToUpper()),

                ChecksLabel = data is BoonCardData ? "CHECK TO ACQUIRE" : "CHECK TO DEFEAT",
                CheckMode = data.checkRequirement.mode
            };

            if (data.checkRequirement.mode == CheckMode.None)
            {
                if (data.checkRequirement.checkSteps.Count > 0)
                    Debug.LogWarning($"{data.name} has CheckMode.None with check steps!");

                viewModel.Check1_Skills = new string[] { "NONE" };
                // We're done!
                return viewModel;
            }

            if (data.checkRequirement.checkSteps.Count == 0)
            {
                Debug.LogError($"{data.name} has CheckMode {data.checkRequirement.mode} but no check steps! Aborting!");
                return null;
            }

            var checkStep1 = data.checkRequirement.checkSteps[0];
            if (checkStep1.category == CheckCategory.Combat)
                viewModel.Check1_Skills = new string[] { "COMBAT" };
            else
                viewModel.Check1_Skills = checkStep1.allowedSkills.Select(s => s.ToString().ToUpper());
            viewModel.Check1_DC = (checkStep1.baseDC + (checkStep1.adventureLevelMult * adventureNumber)).ToString();
            
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
            if (checkStep2.category == CheckCategory.Combat)
                viewModel.Check2_Skills = new string[] { "COMBAT" };
            else
                viewModel.Check2_Skills = checkStep2.allowedSkills.Select(s => s.ToString().ToUpper());
            viewModel.Check2_DC = (checkStep2.baseDC + (checkStep2.adventureLevelMult * adventureNumber)).ToString();

            return viewModel;
        }

        private static Color32 GetPanelColor(PF.CardType cardType)
        {
            switch (cardType)
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
}
