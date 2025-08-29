using System.Collections.Generic;
using UnityEngine;

namespace PACG.Data
{
    public enum CardType
    {
        // Boons
        Ally, Armor, Blessing, Item, Spell, Weapon,
        // Banes
        Barrier, Monster, StoryBane,
        
        // Other
        None, // Used for trait proficiencies.
        
        Character, Location, Scourge
    }
    
    public abstract class CardData : ScriptableObject
    {
        [Header("Unique Identifier")]
        [Tooltip("The unique, programmatic ID for this card.")]
        public string cardID;

        [Header("Base Card Info")]
        public string cardName;
        public CardType cardType;
        public CardType StoryBaneType;
        public int cardLevel;
        public Sprite cardArt;

        [Header("Check(s) to Acquire/Defeat")]
        public CheckRequirement checkRequirement;
        public int rerollThreshold = 0;

        [Header("Powers")]
        [TextArea(3,10)]
        public string powers;
        [TextArea(3,10)]
        public string recovery;
        public List<string> immunities;
        public List<string> vulnerabilities;

        [Header("Traits")]
        public bool IsLoot;
        public List<string> traits;
    }
}
