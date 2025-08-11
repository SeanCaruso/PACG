using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    public class EncounterController : IProcessor
    {
        private readonly PlayerCharacter _pc;
        private readonly CardInstance _card;

        private readonly GameServices _gameServices;
        public GameFlowManager GFM => _gameServices.GameFlow;

        public EncounterController(PlayerCharacter pc, CardInstance card, GameServices gameServices)
        {
            _pc = pc;
            _card = card;

            _gameServices = gameServices;
        }

        public void Execute()
        {
            EncounterContext context = new(_pc, _card);
            _gameServices.Contexts.NewEncounter(context);
            GameEvents.RaiseEncounterStarted(_card);

            // TODO: Add all encounter phases.
            //GFM.QueueNextPhase(new OnEncounterProcessor(_gameServices));
            //GFM.QueueNextPhase(new BeforeActingProcessor(_gameServices));
            GFM.QueueNextPhase(new AttemptChecksProcessor(_gameServices));
            //GFM.QueueNextPhase(new AfterActingProcessor(_gameServices));
            GFM.QueueNextPhase(new ResolveEncounterProcessor(_gameServices));

            GFM.CompleteCurrentPhase();
        }
    }
}
