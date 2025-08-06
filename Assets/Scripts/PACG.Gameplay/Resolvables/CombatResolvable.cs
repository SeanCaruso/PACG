

using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class CombatResolvable : IResolvable
    {
        public LogicRegistry LogicRegistry { get; }
        public PlayerCharacter Character { get; }
        public int Difficulty { get; }

        public CombatResolvable(LogicRegistry logicRegistry, PlayerCharacter character, int difficulty)
        {
            LogicRegistry = logicRegistry;
            this.Character = character;
            Difficulty = difficulty;
        }

        public List<IStagedAction> GetValidActions()
        {
            var allOptions = new List<IStagedAction>();

            foreach (var card in Character.Hand)
            {
                allOptions.AddRange(GetValidActionsForCard(card));
            }
            return allOptions;
        }

        public List<IStagedAction> GetValidActionsForCard(CardInstance card)
        {
            var cardLogic = LogicRegistry.GetPlayableLogic(card);
            return cardLogic?.GetAvailableActions() ?? new();
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
    }
}
