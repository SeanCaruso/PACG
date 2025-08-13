using System.Collections.Generic;
using UnityEngine;

namespace PACG.Data
{
    [CreateAssetMenu(fileName = "AllCards", menuName = "Pathfinder/Card Collection")]
    public class CardCollection : ScriptableObject
    {
        [Header("Banes")]
        public List<CardData> Monsters;
        public List<CardData> Barriers;
        public List<CardData> StoryBanes;

        [Header("Boons")]
        public List<CardData> Weapons;
        public List<CardData> Spells;
        public List<CardData> Armor;
        public List<CardData> Items;
        public List<CardData> Allies;
        public List<CardData> Blessings;
    }
}
