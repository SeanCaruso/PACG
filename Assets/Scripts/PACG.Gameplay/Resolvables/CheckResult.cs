using System.Collections.Generic;
using PACG.Core;

namespace PACG.Gameplay
{
    public class CheckResult
    {
        public int FinalRollTotal { get; set; }
        // ReSharper disable once InconsistentNaming
        public int DC { get; private set; }

        public bool WasSuccess => FinalRollTotal >= DC;
        public int MarginOfSuccess => FinalRollTotal - DC;

        public PlayerCharacter Character { get; private set; }
        public bool IsCombat { get; private set; }
        public Skill Skill { get; private set; }
        public IReadOnlyList<string> Traits { get; private set; }

        public CheckResult(int rollTotal, int dc, PlayerCharacter character, bool isCombat, Skill skill, IReadOnlyList<string> traits)
        {
            FinalRollTotal = rollTotal;
            DC = dc;
            Character = character;
            IsCombat = isCombat;
            Skill = skill;
            Traits = traits;
        }
    }
}
