
using PACG.SharedAPI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace PACG.Gameplay
{
    public enum EncounterPhase
    {
        OnEncounter,
        Evasion,
        Villain_GuardDistant,
        BeforeActing,
        AttemptChecks,
        AfterActing,
        Resolve,
        Avenge,
        Villain_CloseLocation,
        Villain_CheckEscape,
        Villain_Defeat
    }

    public class EncounterProcessor : IProcessor
    {
        // Populated via dependency injection
        private readonly ActionStagingManager _actionStagingManager;
        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlowManager;
        private readonly LogicRegistry _logic;

        private readonly CardInstance _encounteredCard;

        public EncounterProcessor(GameServices gameServices)
        {
            _actionStagingManager = gameServices.ASM;
            _contexts = gameServices.Contexts;
            _gameFlowManager = gameServices.GameFlow;
            _logic = gameServices.Logic;

            _encounteredCard = _contexts.EncounterContext.EncounteredCard;
        }

        public void Execute()
        {
            _contexts.NewEncounter(new(_contexts.TurnContext.CurrentPC, _encounteredCard));
            GameEvents.RaiseEncounterStarted(_encounteredCard);

            // Create and queue up all phases of the encounter.
            var encounterPhases = new List<IProcessor>
            {
                new BeforeActingPhase(_contexts, _logic),
                new AttemptChecksPhase(_contexts, _logic),
                new AfterActingPhase(_contexts, _logic),
                new ResolveEncounterPhase(_contexts, _logic)
            };

            _gameFlowManager.QueueProcessors(encounterPhases);
        }
    }
}
