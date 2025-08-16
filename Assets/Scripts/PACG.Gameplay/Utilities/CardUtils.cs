using UnityEngine;

namespace PACG.Gameplay
{
    public static class CardUtils
    {
        private static bool _isInitialized;
        private static int _adventureNumber = 1;
        public static void Initialize(int adventureNumber)
        {
            _isInitialized = true;
            _adventureNumber = adventureNumber;
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

            return baseDc + adventureLevelMult * _adventureNumber;
        }
    }
}
