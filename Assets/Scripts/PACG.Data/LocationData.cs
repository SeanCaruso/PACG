using System.Collections.Generic;
using UnityEngine;

namespace PACG.Data
{
    [CreateAssetMenu(fileName = "LocationName", menuName = "Pathfinder/Location Card")]
    public class LocationData : ScriptableObject
    {
        [Header("Basic Info")]
        public string LocationName;
        public int Level;

        [Header("Powers")]
        [TextArea(2, 3)]
        public string Power_AtLocation;
        [TextArea(2, 3)]
        public string Power_ToClose;
        [TextArea(2, 3)]
        public string Power_WhenClosed;

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
