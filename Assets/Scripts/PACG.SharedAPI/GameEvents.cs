using PACG.Gameplay;
using System;
using System.Collections.Generic;

namespace PACG.SharedAPI
{
    public static class GameEvents
    {
        // Turn phase events
        public static event Action<TurnContext> TurnStateChanged;
        public static void RaiseTurnStateChanged(TurnContext context) => TurnStateChanged?.Invoke(context);

        public static event Action<CardInstance> HourChanged;
        public static void RaiseHourChanged(CardInstance hourCard) => HourChanged?.Invoke(hourCard);

        public static event Action<CardInstance> EncounterStarted;
        public static void RaiseEncounterStarted(CardInstance encounteredCard) => EncounterStarted?.Invoke(encounteredCard);

        // Card staging events
        public static event Action<StagedActionsState> StagedActionsStateChanged;
        public static void RaiseStagedActionsStateChanged(StagedActionsState stagedActionsState) =>
            StagedActionsStateChanged?.Invoke(stagedActionsState);

        // Card display events
        public static event Action<CardInstance> CardLocationChanged;
        public static void RaiseCardLocationChanged(CardInstance cardInstance) => CardLocationChanged?.Invoke(cardInstance);

        public static event Action<List<CardInstance>> CardLocationsChanged;
        public static void RaiseCardLocationsChanged(List<CardInstance> cards) => CardLocationsChanged?.Invoke(cards);
    }
}
