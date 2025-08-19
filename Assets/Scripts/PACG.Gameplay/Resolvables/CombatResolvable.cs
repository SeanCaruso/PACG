using System.Collections.Generic;

namespace PACG.Gameplay
{
    public class CombatResolvable : BaseResolvable, ICheckResolvable
    {
        public ICard Card { get; }
        public PlayerCharacter Character { get; }
        private readonly List<PF.Skill> _skills = new() { PF.Skill.Strength, PF.Skill.Melee };
        public IReadOnlyList<PF.Skill> Skills => _skills;
        public int Difficulty { get; }

        public CombatResolvable(CardInstance card, PlayerCharacter character, int difficulty)
        {
            Card = card;
            Character = character;
            Difficulty = difficulty;
        }

        public override bool CanCommit(List<IStagedAction> actions)
        {
            foreach (var action in actions)
            {
                if (action is PlayCardAction playAction && playAction.IsCombat)
                    return true;
            }
            return false;
        }

        public override IProcessor CreateProcessor(GameServices gameServices) => new CheckController(this, gameServices);
    }
}
