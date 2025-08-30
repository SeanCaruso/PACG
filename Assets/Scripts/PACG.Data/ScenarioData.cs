using UnityEngine;

namespace PACG.Data
{
    [CreateAssetMenu(fileName = "ScenarioName", menuName = "Pathfinder/Scenario")]
    public class ScenarioData : ScriptableObject
    {
        [System.Serializable]
        public struct Location
        {
            public int NumPcs;
            public LocationData LocationData;
        }

        [System.Serializable]
        public struct StoryBane
        {
            public CardData CardData;
            public string CustomName;
            public bool IsClosing;
        }

        [Header("Scenario Name")]
        public string Name;
        public string ID;

        [Header("Setup")]
        [TextArea(3, 6)]
        public string Setup;
        
        [Header("Locations")]
        public Location[] Locations;

        [Header("During This Scenario")]
        [TextArea(3, 6)]
        public string DuringScenario;
        public bool IsDuringPowerActivated;
        public Sprite DuringScenarioPowerEnabled;
        public Sprite DuringScenarioPowerDisabled;

        [Header("Story Banes")]
        public StoryBane[] Dangers;
        public StoryBane Villain;
        public StoryBane[] Henchmen;

        [Header("Reward")]
        [TextArea(1, 3)]
        public string Reward;

    }
}
