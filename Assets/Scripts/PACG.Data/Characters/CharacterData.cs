using System;
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Data
{
    [System.Serializable]
    public struct AttributeSkill
    {
        public PF.Skill attribute;
        public int die;
        public int bonus;
    }

    [System.Serializable]
    public struct Skill
    {
        public PF.Skill skill;
        public PF.Skill attribute;
        public int bonus;
    }

    [System.Serializable]
    public struct FavoredCard
    {
        public PF.CardType cardType;
        public string trait;
    }

    [System.Serializable]
    public struct CharacterPower : IEquatable<CharacterPower>
    {
        public bool isActivated;
        public Sprite spriteEnabled;
        public Sprite spriteDisabled;
        [TextArea(2, 3)]
        public string text;

        public bool Equals(CharacterPower other)
        {
            return isActivated == other.isActivated &&
                   Equals(spriteEnabled, other.spriteEnabled) &&
                   Equals(spriteDisabled, other.spriteDisabled) &&
                   text == other.text;
        }

        public override bool Equals(object obj)
        {
            return obj is CharacterPower other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(isActivated, spriteEnabled, spriteDisabled, text);
        }
    }

    [CreateAssetMenu(fileName = "CharacterName", menuName = "Pathfinder/Character Card")]
    public class CharacterData : ScriptableObject
    {
        public string characterName;

        [Header("Skills")]
        public List<AttributeSkill> attributes;
        public List<Skill> skills;

        [Header("Other Basic Info")]
        public int handSize;
        public List<PF.CardType> proficiencies;
        public List<string> traits;

        [Header("Powers")]
        public List<CharacterPower> powers;

        [Header("Deck List")]
        public int weapons;
        public int spells;
        public int armors;
        public int items;
        public int allies;
        public int blessings;
        public List<FavoredCard> favoredCards;
    }
}
