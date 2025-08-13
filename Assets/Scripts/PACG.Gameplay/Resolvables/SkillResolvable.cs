using System;
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class SkillResolvable : BaseResolvable, ICheckResolvable
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

        public override bool IsResolved(List<IStagedAction> actions)
        {
            throw new NotImplementedException();
        }

        public override IProcessor CreateProcessor(GameServices gameServices) => new CheckController(this, gameServices);
    }
}
