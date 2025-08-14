using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class CombatResolvable : BaseResolvable, ICheckResolvable
    {
        public PlayerCharacter Character { get; }
        public int Difficulty { get; }

        public CombatResolvable(PlayerCharacter character, int difficulty)
        {
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
