using System;
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Data
{
    public enum LocationPowerType
    {
        AtLocation,
        ToClose,
        WhenClosed
    }
    
    [Serializable]
    public struct LocationPower : IEquatable<LocationPower>
    {
        public LocationPowerType PowerType;
        public bool IsActivated;
        public Sprite SpriteEnabled;
        public Sprite SpriteDisabled;
        [TextArea(2, 3)]
        public string Text;

        public Action OnActivate;

        public bool Equals(LocationPower other)
        {
            return PowerType == other.PowerType
                   && IsActivated == other.IsActivated
                   && Equals(SpriteEnabled, other.SpriteEnabled)
                   && Equals(SpriteDisabled, other.SpriteDisabled)
                   && Text == other.Text;
        }

        public override bool Equals(object obj)
        {
            return obj is LocationPower other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)PowerType, IsActivated, SpriteEnabled, SpriteDisabled, Text);
        }
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
