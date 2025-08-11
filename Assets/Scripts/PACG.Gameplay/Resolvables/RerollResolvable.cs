using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class RerollResolvable : IResolvable
    {
        public PlayerCharacter Character { get; }

        public RerollResolvable(PlayerCharacter pc, CheckContext checkContext)
        {
            Character = pc;

            // Default option is to not reroll.
            checkContext.ContextData["doReroll"] = false;
        }

        public List<IStagedAction> GetAdditionalActionsForCard(CardInstance card)
        {
            // Reroll resolvables don't add any additional actions beyond what cards provide
            return new List<IStagedAction>();
        }

        public bool IsResolved(List<IStagedAction> actions) => true; // We can always resolve by skipping.

        public IProcessor CreateProcessor(GameServices gameServices)
        {
            // If something set the "doReroll" context data to true, process the roll again.
            if ((bool)gameServices.Contexts.CheckContext.ContextData["doReroll"] == true)
            {
                Debug.Log($"[{GetType().Name}] User chose to reroll - returning a processor.");
                return new Check_RollDiceProcessor(gameServices);
            }

            // No reroll - continue flow as normal.
            return null;
        }
    }
}