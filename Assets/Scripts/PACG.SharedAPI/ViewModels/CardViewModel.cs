using System.Collections.Generic;
using System.Linq;
using PACG.Core;
using PACG.Gameplay;
using UnityEngine;

namespace PACG.SharedAPI
{
    public class CardViewModel
    {
        public CardViewModel(CardInstance cardInstance)
        {
            CardInstance = cardInstance;
        }
        
        // The underlying CardInstance
        public CardInstance CardInstance { get; }
        
        // Ready-to-display strings
        public string Name => CardInstance.Data.cardName.ToUpper();
        public string Type => CardInstance.Data.cardType.ToString().ToUpper();
        public string Level => $"{CardInstance.Data.cardLevel}";
        public string PowersText => StringUtils.ReplaceAdventureLevel(CardInstance.Data.powers, CardUtils.AdventureNumber);
        public string RecoveryText => StringUtils.ReplaceAdventureLevel(CardInstance.Data.recovery, CardUtils.AdventureNumber);

        // Ready-to-use values
        public Color32 PanelColor { get; set; }
        public IEnumerable<string> Traits => CardInstance.Data.traits.Select(trait => trait.ToUpper());

        // Check Section 1
        public string ChecksLabel => CardInstance.Data is BoonCardData ? "CHECK TO ACQUIRE" : "CHECK TO DEFEAT";
        public IEnumerable<string> Check1Skills { get; set; }
        public string Check1Dc { get; set; } = "";

        // Check Section 2
        public bool ShowCheck2 => CheckMode is CheckMode.Choice or CheckMode.Sequential;
        public CheckMode CheckMode => CardInstance.Data.checkRequirement.mode; // For showing 'OR'/'THEN'
        public IEnumerable<string> Check2Skills { get; set; }
        public string Check2Dc { get; set; } = "";
    }
}
