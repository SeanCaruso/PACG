using PACG.SharedAPI.States;
using System;
using System.Collections.Generic;

namespace PACG.Services.Core
{
    public static class GameEvents
    {
        // Card staging events
        public static event Action<IStagedAction> ActionStaged;
        public static void RaiseActionStaged(IStagedAction action) => ActionStaged?.Invoke(action);

        public static event Action<IStagedAction> ActionUnstaged;
        public static void RaiseActionUnstaged(IStagedAction action) => ActionUnstaged?.Invoke(action);

        public static event Action<List<IStagedAction>> ActionsCommitted;
        public static void RaiseActionsCommitted(List<IStagedAction> actions) => ActionsCommitted?.Invoke(actions);

        public static event Action StagingCancelled;
        public static void RaiseStagingCancelled() => StagingCancelled?.Invoke();

        public static event Action<StagedActionsState> StagedActionsStateChanged;
        public static void RaiseStagedActionsStateChanged(StagedActionsState stagedActionsState) =>
            StagedActionsStateChanged?.Invoke(stagedActionsState);

        // Card location events
        public static event Action<CardInstance> CardLocationChanged;
        public static void RaiseCardLocationChanged(CardInstance cardInstance) => CardLocationChanged?.Invoke(cardInstance);
    }
}
