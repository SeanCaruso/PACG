
using PACG.Data;

namespace PACG.Core
{
    public enum ActionType { Banish, Bury, Discard, Display, Draw, Recharge, Reload, Reveal }

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
        Craft,
        Diplomacy,
        Disable,
        Divine,
        Fortitude,
        Knowledge,
        Melee,
        Perception,
        Ranged,
        Stealth,
        Survival,
    }
    
    // ReSharper disable once InconsistentNaming
    public static class PF
    {

        public static string ToString(CardType type)
        {
            return type switch
            {
                CardType.StoryBane => "Story Bane",
                _ => type.ToString()
            };
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

        public enum TargetingRule { CurrentPlayer, RandomPlayer, LocalPlayer, EachLocalPlayer, RandomLocalPlayer }
    }
}
