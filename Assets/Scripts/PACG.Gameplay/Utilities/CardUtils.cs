using System.Collections.Generic;
using System.Linq;
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
        /// Convenience function for a simple skill check.
        /// </summary>
        /// <param name="baseDc"></param>
        /// <param name="skills"></param>
        /// <returns></returns>
        public static CheckRequirement SkillCheck(int baseDc, params PF.Skill[] skills)
        {
            return new CheckRequirement
            {
                mode = CheckMode.Single,
                checkSteps = new List<CheckStep>{ new ()
                {
                    category = CheckCategory.Skill,
                    baseDC = baseDc,
                    allowedSkills = skills.ToList()
                } }
            };
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
