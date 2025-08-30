using System;
using System.Collections.Generic;
using PACG.Core;
using PACG.Data;
using PACG.Gameplay;

namespace PACG.SharedAPI
{
    public static class GameEvents
    {
        // Scenario events
        public static event Action<GameServices> ScenarioHasPower;
        public static void RaiseScenarioHasPower(GameServices gameServices) => ScenarioHasPower?.Invoke(gameServices);

        public static event Action<bool> ScenarioPowerEnabled;
        public static void RaiseScenarioPowerEnabled(bool enabled) => ScenarioPowerEnabled?.Invoke(enabled);

        // Turn phase events
        public static event Action TurnStateChanged;
        public static void RaiseTurnStateChanged() => TurnStateChanged?.Invoke();

        public static event Action<CardInstance> HourChanged;
        public static void RaiseHourChanged(CardInstance hourCard) => HourChanged?.Invoke(hourCard);

        public static event Action<Location> LocationChanged;
        public static void RaiseLocationChanged(Location location) => LocationChanged?.Invoke(location);

        public static event Action<CardInstance> EncounterStarted;

        public static void RaiseEncounterStarted(CardInstance encounteredCard) =>
            EncounterStarted?.Invoke(encounteredCard);

        public static event Action EncounterEnded;
        public static void RaiseEncounterEnded() => EncounterEnded?.Invoke();

        // Card staging events
        public static event Action<StagedActionsState> StagedActionsStateChanged;

        public static void RaiseStagedActionsStateChanged(StagedActionsState stagedActionsState) =>
            StagedActionsStateChanged?.Invoke(stagedActionsState);

        // Card display events
        public static event Action<CardInstance> CardLocationChanged;

        public static void RaiseCardLocationChanged(CardInstance cardInstance) =>
            CardLocationChanged?.Invoke(cardInstance);

        public static event Action<List<CardInstance>> CardLocationsChanged;
        public static void RaiseCardLocationsChanged(List<CardInstance> cards) => CardLocationsChanged?.Invoke(cards);

        // Location events
        public static event Action<LocationPower, bool> LocationPowerEnabled;

        public static void RaiseLocationPowerEnabled(LocationPower power, bool enabled) =>
            LocationPowerEnabled?.Invoke(power, enabled);

        // Player Character events
        public static event Action<PlayerCharacter> PlayerCharacterChanged;
        public static void RaisePlayerCharacterChanged(PlayerCharacter pc) => PlayerCharacterChanged?.Invoke(pc);

        public static event Action<CharacterPower, bool> PlayerPowerEnabled;

        public static void RaisePlayerPowerEnabled(CharacterPower power, bool enabled) =>
            PlayerPowerEnabled?.Invoke(power, enabled);

        public static event Action<int> PlayerDeckCountChanged;
        public static void RaisePlayerDeckCountChanged(int count) => PlayerDeckCountChanged?.Invoke(count);

        // Special Resolvable events
        public static event Action<PlayerChoiceResolvable> PlayerChoiceEvent;

        public static void RaisePlayerChoiceEvent(PlayerChoiceResolvable resolvable) =>
            PlayerChoiceEvent?.Invoke(resolvable);

        // General game status events
        public static event Action<string> SetStatusTextEvent;
        public static void SetStatusText(string text) => SetStatusTextEvent?.Invoke(text);

        public static event Action<DicePool> DicePoolChanged;
        public static void RaiseDicePoolChanged(DicePool dicePool) => DicePoolChanged?.Invoke(dicePool);
    }
}
