using System.Collections.Generic;
using UnityEngine;

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

    [Header("Deck List")]
    public int weapons;
    public int spells;
    public int armors;
    public int items;
    public int allies;
    public int blessings;
    public List<FavoredCard> favoredCards;
}
