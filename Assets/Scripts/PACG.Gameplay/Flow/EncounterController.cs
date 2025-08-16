using PACG.SharedAPI;

namespace PACG.Gameplay
{
    public class EncounterController : IProcessor, IPhaseController
    {
        private readonly PlayerCharacter _pc;
        private readonly CardInstance _card;

        // Dependency injections
        private readonly GameFlowManager _gameFlow;
        private readonly GameServices _gameServices;

        public EncounterController(PlayerCharacter pc, CardInstance card, GameServices gameServices)
        {
            _pc = pc;
            _card = card;

            _gameFlow = gameServices.GameFlow;
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
            _gameFlow.QueueNextProcessor(new AttemptChecksProcessor(_gameServices));
            //GFM.QueueNextPhase(new AfterActingProcessor(_gameServices));
            _gameFlow.QueueNextProcessor(new ResolveEncounterProcessor(_gameServices));

            _gameFlow.CompleteCurrentPhase();
        }
    }
}
