using UnityEngine;

namespace PACG.Gameplay
{
    public static class CardUtils
    {
        private static bool _isInitialized;
        public static int AdventureNumber { get; private set; } = 1;
        public static void Initialize(int adventureNumber)
        {
            _isInitialized = true;
            AdventureNumber = adventureNumber;
        }

        /// <summary>
        /// Returns the modified DC based on the current adventure number. INITIALIZE MUST BE CALLED BEFORE THIS!
        /// </summary>
        /// <param name="baseDc">Base DC</param>
        /// <param name="adventureLevelMult">Adventure level multiplier (#)</param>
        /// <returns></returns>
        public static int GetDc(int baseDc, int adventureLevelMult)
        {
            if (!_isInitialized) Debug.LogError("CardUtils MUST be initialized!!!");

            return baseDc + adventureLevelMult * AdventureNumber;
        }
    }
}
