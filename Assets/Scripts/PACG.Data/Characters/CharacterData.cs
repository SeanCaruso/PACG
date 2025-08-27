using System;
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Data
{
    [Serializable]
    public struct AttributeSkill
    {
        public PF.Skill Attribute;
        public int Die;
        public int Bonus;
    }

    [Serializable]
    public struct PcSkill
    {
        public PF.Skill Skill;
        public PF.Skill Attribute;
        public int Bonus;
    }

    [Serializable]
    public struct Proficiency
    {
        public PF.CardType CardType;
        public string Trait;
    }

    [Serializable]
    public struct FavoredCard
    {
        public PF.CardType CardType;
        public string Trait;
    }

    [Serializable]
    public struct CharacterPower : IEquatable<CharacterPower>
    {
        public bool IsActivated;
        public Sprite SpriteEnabled;
        public Sprite SpriteDisabled;
        [TextArea(2, 3)]
        public string Text;
        
        public Action OnActivate;

        public bool Equals(CharacterPower other)
        {
            return IsActivated == other.IsActivated &&
                   Equals(SpriteEnabled, other.SpriteEnabled) &&
                   Equals(SpriteDisabled, other.SpriteDisabled) &&
                   Text == other.Text;
        }

        public override bool Equals(object obj)
        {
            return obj is CharacterPower other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IsActivated, SpriteEnabled, SpriteDisabled, Text);
        }
    }

    [CreateAssetMenu(fileName = "CharacterName", menuName = "Pathfinder/Character Card")]
    public class CharacterData : ScriptableObject
    {
        public string CharacterName;

        [Header("Skills")]
        public List<AttributeSkill> Attributes;
        public List<PcSkill> Skills;

        [Header("Other Basic Info")]
        public int HandSize;
        public List<Proficiency> Proficiencies;
        public List<string> Traits;

        [Header("Powers")]
        public List<CharacterPower> Powers;

        [Header("Deck List")]
        public int Weapons;
        public int Spells;
        public int Armors;
        public int Items;
        public int Allies;
        public int Blessings;
        public List<FavoredCard> FavoredCards;
    }
}
