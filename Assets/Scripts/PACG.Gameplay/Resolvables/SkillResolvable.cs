using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PACG.Gameplay
{
    public class SkillResolvable : BaseResolvable, ICheckResolvable
    {
        public PlayerCharacter Character { get; }
        public List<PF.Skill> Skills { get; }
        public int Difficulty { get; }

        public SkillResolvable(PlayerCharacter character, int difficulty, params PF.Skill[] skills)
        {
            Character = character;
            Skills = skills.ToList();
            Difficulty = difficulty;
        }

        public override bool IsResolved(List<IStagedAction> actions)
        {
            throw new NotImplementedException();
        }

        public override IProcessor CreateProcessor(GameServices gameServices) => new CheckController(this, gameServices);
    }
}
