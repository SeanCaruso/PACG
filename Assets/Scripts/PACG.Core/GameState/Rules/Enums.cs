
public static class PF
{

    public enum ActionType { Banish, Bury, Discard, Display, Draw, Recharge, Reload, Reveal }

    public enum CardType
    {
        // Boons
        Ally, Armor, Blessing, Item, Spell, Weapon,
        // Banes
        Barrier, Monster, StoryBane
    }
    public static string S(CardType type)
    {
        switch(type)
        {
            case CardType.StoryBane:
                return "Story Bane";
            default:
                return type.ToString();
        }
    }
    public static bool IsBane(CardType cardType)
    {
        return cardType switch
        {
            CardType.Barrier or CardType.Monster or CardType.StoryBane => true,
            _ => false,
        };
    }
    public static bool IsBoon(CardType cardType) => !IsBane(cardType);

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