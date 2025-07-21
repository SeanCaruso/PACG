
namespace PF
{

    public enum ActionType { Draw, Reveal, Display, Reload, Recharge, Discard, Bury, Banish }

    public enum CardType
    {
        // Boons
        Ally, Armor, Blessing, Item, Spell, Weapon,
        // Banes
        Barrier, Monster
    }
    public enum Skill
    {
        Strength,
        Dexterity,
        Constitution,
        Intelligence,
        Wisdom,
        Charisma,

        Acrobatics,
        Arcane,
        Diplomacy,
        Divine,
        Fortitude,
        Knowledge,
        Melee,
        Perception,
        Ranged,
        Stealth,
        Survival
    }

    public enum TargetingRule { CurrentPlayer, RandomPlayer, LocalPlayer, EachLocalPlayer, RandomLocalPlayer }
}