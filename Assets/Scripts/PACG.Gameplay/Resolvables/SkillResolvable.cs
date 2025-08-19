using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public class SkillResolvable : BaseResolvable, ICheckResolvable
    {
        public ICard Card { get; }
        public PlayerCharacter Character { get; }
        public IReadOnlyList<PF.Skill> Skills { get; }
        public int Difficulty { get; }

        public SkillResolvable(ICard card, PlayerCharacter character, int difficulty, params PF.Skill[] skills)
        {
            Card = card;
            Character = character;
            Skills = skills.ToList();
            Difficulty = difficulty;
        }

        public override bool CanCommit(List<IStagedAction> actions) => true;

        public override IProcessor CreateProcessor(GameServices gameServices) => new CheckController(this, gameServices);
    }
}
