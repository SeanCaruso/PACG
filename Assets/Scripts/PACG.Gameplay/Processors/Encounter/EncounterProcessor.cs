
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
        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlowManager;
        private readonly LogicRegistry _logic;

        private readonly CardInstance _encounteredCard;

        public EncounterProcessor(GameServices gameServices, CardInstance exploredCard)
        {
            _contexts = gameServices.Contexts;
            _gameFlowManager = gameServices.GameFlow;
            _logic = gameServices.Logic;

            _encounteredCard = exploredCard;
        }

        public void Execute()
        {
            _contexts.NewEncounter(new(_contexts.TurnContext.CurrentPC, _encounteredCard));
            GameEvents.RaiseEncounterStarted(_encounteredCard);

            var cardLogic = _logic.GetEncounterLogic(_encounteredCard);
            if (cardLogic == null) Debug.LogError($"[EncounterProcessor] Unable to find encounter logic for {_encounteredCard.Data.name}");

            // The card logic is responsible for creating resolvables and adding them to GFM.
            cardLogic?.Execute();
        }
    }
}
