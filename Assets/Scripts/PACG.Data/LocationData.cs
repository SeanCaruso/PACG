using System;
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Data
{
    [Serializable]
    public struct LocationPower
    {
        public bool IsActivated;
        public Sprite SpriteEnabled;
        public Sprite SpriteDisabled;
        [TextArea(2, 3)]
        public string Text;

        public Action OnActivate;
    }
    
    [CreateAssetMenu(fileName = "LocationName", menuName = "Pathfinder/Location Card")]
    public class LocationData : ScriptableObject
    {
        [Header("Basic Info")]
        public string LocationName;
        public int Level;

        [Header("Powers")]
        public LocationPower AtLocationPower;
        public LocationPower ToClosePower;
        public LocationPower WhenClosedPower;

        [Header("Card List")]
        public int NumMonsters;
        public int NumBarriers;
        public int NumWeapons;
        public int NumSpells;
        public int NumArmors;
        public int NumItems;
        public int NumAllies;
        public int NumBlessings;

        [Header("Traits")]
        public List<string> Traits;
    }
}
