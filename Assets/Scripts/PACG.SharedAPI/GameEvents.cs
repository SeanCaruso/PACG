using PACG.Gameplay;
using System;
using System.Collections.Generic;

namespace PACG.SharedAPI
{
    public static class GameEvents
    {
        // Turn phase events
        public static event Action TurnStateChanged;
        public static void RaiseTurnStateChanged() => TurnStateChanged?.Invoke();

        public static event Action<CardInstance> HourChanged;
        public static void RaiseHourChanged(CardInstance hourCard) => HourChanged?.Invoke(hourCard);

        public static event Action<CardInstance> EncounterStarted;
        public static void RaiseEncounterStarted(CardInstance encounteredCard) => EncounterStarted?.Invoke(encounteredCard);

        public static event Action EncounterEnded;
        public static void RaiseEncounterEnded() => EncounterEnded?.Invoke();

        // Card staging events
        public static event Action<StagedActionsState> StagedActionsStateChanged;
        public static void RaiseStagedActionsStateChanged(StagedActionsState stagedActionsState) =>
            StagedActionsStateChanged?.Invoke(stagedActionsState);

        // Card display events
        public static event Action<CardInstance> CardLocationChanged;
        public static void RaiseCardLocationChanged(CardInstance cardInstance) => CardLocationChanged?.Invoke(cardInstance);

        public static event Action<List<CardInstance>> CardLocationsChanged;
        public static void RaiseCardLocationsChanged(List<CardInstance> cards) => CardLocationsChanged?.Invoke(cards);

        // Player Character events
        public static event Action<PlayerCharacter> PlayerCharacterChanged;
        public static void RaisePlayerCharacterChanged(PlayerCharacter pc) => PlayerCharacterChanged?.Invoke(pc);

        public static event Action<CharacterPower, bool, IResolvable> PlayerPowerEnabled;
        public static void RaisePlayerPowerEnabled(CharacterPower power, bool enabled, IResolvable powerResolvable = null) =>
            PlayerPowerEnabled?.Invoke(power, enabled, powerResolvable);

        public static event Action<int> PlayerDeckCountChanged;
        public static void RaisePlayerDeckCountChanged(int count) => PlayerDeckCountChanged?.Invoke(count);

        // General game status events
        public static event Action<string> SetStatusTextEvent;
        public static void SetStatusText(string text) => SetStatusTextEvent?.Invoke(text);


    }
}
