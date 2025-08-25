using System.Collections.Generic;

namespace PACG.Gameplay
{
    public enum TurnPhase
    {
        TurnStart,          // Advance the hour, start-of-turn effects
        TurnActions,        // Optional actions: give/move/explore (in that order)
        CloseLocation,      // Attempt to close the location (if empty)
        EndOfTurnEffects,   // Apply end-of-turn effects
        Recovery,           // Deal with recovery pile
        Reset,              // Make optional discards and satisfy hand size 
        None                // Stop processing!
    }

    public class TurnContext
    {
        public PlayerCharacter Character { get; }
        public Location Location => Character.Location;

        public TurnPhase CurrentPhase { get; set; } = TurnPhase.TurnStart;

        public CardInstance HourCard { get; set; } // Set in TurnStartProcessor

        public bool CanGive { get; set; }
        public bool CanMove { get; set; }
        public bool CanFreelyExplore { get; set; }
        public bool CanCloseLocation { get; set; }
        public bool CanEndTurn { get; set; }

        public bool ForceEndTurn { get; set; } = false;

        public List<IExploreEffect> ExploreEffects { get; } = new();

        public TurnContext(PlayerCharacter currentPC)
        {
            Character = currentPC;
        }
        
        public void AddExploreEffect(IExploreEffect effect) => ExploreEffects.Add(effect);
    }
}
