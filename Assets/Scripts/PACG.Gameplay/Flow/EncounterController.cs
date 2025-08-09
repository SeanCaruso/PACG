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

            GFM.QueueNextPhase(new OnEncounterProcessor(context, _gameServices));
            GFM.QueueNextPhase(new BeforeActingProcessor(context, _gameServices));
            GFM.QueueNextPhase(new AttemptChecksProcessor(context, _gameServices));
            GFM.QueueNextPhase(new AfterActingProcessor(context, _gameServices));
            GFM.QueueNextPhase(new ResolveEncounterProcessor(context, _gameServices));

            GFM.CompleteCurrentPhase();
        }
    }
}
