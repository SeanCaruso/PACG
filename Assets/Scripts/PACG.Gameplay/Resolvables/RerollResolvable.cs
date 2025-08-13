using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class RerollResolvable : BaseResolvable
    {
        public PlayerCharacter Character { get; }

        public RerollResolvable(PlayerCharacter pc, CheckContext checkContext)
        {
            Character = pc;

            // Default option is to not reroll.
            checkContext.ContextData["doReroll"] = false;
        }

        public override IProcessor CreateProcessor(GameServices gameServices)
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