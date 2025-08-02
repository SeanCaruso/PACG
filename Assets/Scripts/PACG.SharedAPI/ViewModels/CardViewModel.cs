using System.Collections.Generic;
using UnityEngine;

namespace PACG.SharedAPI.ViewModels
{
    public class CardViewModel
    {
        // Ready-to-display strings
        public string Name { get; set; }
        public string Type { get; set; }
        public string Level { get; set; }
        public string PowersText { get; set; }
        public string RecoveryText { get; set; }

        // Ready-to-use values
        public Color32 PanelColor { get; set; }
        public IEnumerable<string> Traits { get; set; }

        // Check Section 1
        public string ChecksLabel { get; set; }
        public IEnumerable<string> Check1_Skills { get; set; }
        public string Check1_DC { get; set; } = "";

        // Check Section 2
        public bool ShowCheck2 { get; set; } = false;
        public CheckMode CheckMode { get; set; } // For showing 'OR'/'THEN'
        public IEnumerable<string> Check2_Skills { get; set; }
        public string Check2_DC { get; set; } = "";
    }
}
