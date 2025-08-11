using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class CombatResolvable : IResolvable, ICheckResolvable
    {
        public PlayerCharacter Character { get; }
        public int Difficulty { get; }

        public CombatResolvable(LogicRegistry logicRegistry, PlayerCharacter character, int difficulty)
        {
            Character = character;
            Difficulty = difficulty;
        }

        public List<IStagedAction> GetValidActions()
        {
            var allOptions = new List<IStagedAction>();
            foreach (var card in Character.Hand) allOptions.AddRange(card.GetAvailableActions());

            return allOptions;
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
