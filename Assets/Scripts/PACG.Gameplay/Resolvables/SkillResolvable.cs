using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class SkillResolvable : IResolvable, ICheckResolvable
    {
        public LogicRegistry LogicRegistry { get; }
        public PlayerCharacter Character { get; }
        public int Difficulty { get; }

        public SkillResolvable(LogicRegistry logicRegistry, PlayerCharacter character, int difficulty)
        {
            LogicRegistry = logicRegistry;
            Character = character;
            Difficulty = difficulty;
        }

        public List<IStagedAction> GetAdditionalActionsForCard(CardInstance card)
        {
            // Skill resolvables don't add any additional actions beyond what cards provide
            return new List<IStagedAction>();
        }

        public bool IsResolved(List<IStagedAction> actions)
        {
            return false;
        }

        public IProcessor CreateProcessor(GameServices gameServices)
        {
            return new CheckController(this, gameServices);
        }
    }
}
