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
