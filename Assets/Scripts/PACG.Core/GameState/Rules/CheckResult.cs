using System.Collections.Generic;

public class CheckResult
{
    public int FinalRollTotal {  get; set; }
    public int DC {  get; private set; }

    public bool WasSuccess => FinalRollTotal >= DC;
    public int MarginOfSuccess => FinalRollTotal - DC;

    public PlayerCharacter Character { get; private set; }
    public PF.Skill Skill { get; private set; }
    public IReadOnlyList<string> Traits { get; private set; }

    public CheckResult(int rollTotal, int dc, PlayerCharacter character,  PF.Skill skill, IReadOnlyList<string> traits)
    {
        FinalRollTotal = rollTotal;
        DC = dc;
        Character = character;
        Skill = skill;
        Traits = traits;
    }
}
