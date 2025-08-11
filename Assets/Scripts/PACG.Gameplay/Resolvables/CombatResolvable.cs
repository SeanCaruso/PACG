using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class CombatResolvable : IResolvable, ICheckResolvable
    {
        public PlayerCharacter Character { get; }
        public int Difficulty { get; }

        public CombatResolvable(PlayerCharacter character, int difficulty)
        {
            Character = character;
            Difficulty = difficulty;
        }

        public List<IStagedAction> GetAdditionalActionsForCard(CardInstance card)
        {
            // Combat resolvables don't add any additional actions beyond what cards provide
            return new List<IStagedAction>();
        }

        public bool IsResolved(List<IStagedAction> actions)
        {
            foreach (var action in actions)
            {
                if (action is PlayCardAction playAction && playAction.IsCombat)
                    return true;
            }
            return false;
        }

        public IProcessor CreateProcessor(GameServices gameServices) => new CheckController(this, gameServices);
    }
}
