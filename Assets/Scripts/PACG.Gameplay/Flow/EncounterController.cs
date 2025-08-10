using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    public class EncounterController : IProcessor
    {
        private readonly PlayerCharacter _pc;
        private readonly CardLogicBase _cardLogic;

        private readonly GameServices _gameServices;
        public GameFlowManager GFM => _gameServices.GameFlow;

        public EncounterController(PlayerCharacter pc, CardLogicBase cardLogic, GameServices gameServices)
        {
            _pc = pc;
            _cardLogic = cardLogic;

            _gameServices = gameServices;
        }

        public void Execute()
        {
            EncounterContext context = new(_pc, _cardLogic);
            _gameServices.Contexts.NewEncounter(context);
            GameEvents.RaiseEncounterStarted(_cardLogic.Card);

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
