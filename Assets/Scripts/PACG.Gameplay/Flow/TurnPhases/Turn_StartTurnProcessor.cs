using System.Linq;
using PACG.SharedAPI;

namespace PACG.Gameplay
{
    public class Turn_StartTurnProcessor : BaseProcessor
    {
        // Dependency injection
        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlow;
        private readonly GameServices _gameServices;

        public Turn_StartTurnProcessor(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
            _gameServices = gameServices;
        }

        protected override void OnExecute()
        {
            if (_contexts.TurnContext == null) return;
            _contexts.TurnContext.CurrentPhase = TurnPhase.TurnStart;

            // Only continue once we finished handling start-of-turn powers.
            if (HandledStartOfTurnPowers())
                return;

            var pc = _contexts.TurnContext.Character;

            // TODO: Allow the user to pick the order they do start-of-turn actions.
            if (pc.ActiveScourges.Contains(ScourgeType.Wounded))
                ScourgeRules.HandleWoundedDeckDiscard(pc, _gameServices);

            // Set initial availability of turn actions
            _contexts.TurnContext.CanGive = _contexts.TurnContext.Character.LocalCharacters.Count > 1;
            _contexts.TurnContext.CanMove = _contexts.GameContext?.Locations.Count > 1;
            _contexts.TurnContext.CanFreelyExplore = _contexts.TurnPcLocation?.Count > 0;
            _contexts.TurnContext.CanCloseLocation = _contexts.TurnPcLocation?.Count == 0;

            if (pc.ActiveScourges.Contains(ScourgeType.Entangled))
                _contexts.TurnContext.CanMove = false;

            if (pc.ActiveScourges.Contains(ScourgeType.Exhausted))
                ScourgeRules.PromptForExhaustedRemoval(pc, _gameServices);

            GameEvents.RaiseTurnStateChanged(); // Update turn action button states.

            _contexts.TurnContext.CurrentPhase = TurnPhase.TurnActions;
        }

        /// <summary>
        /// Finds and handles any start-of-turn powers.
        /// </summary>
        /// <returns>true if a power was found (and another processor was started), false otherwise</returns>
        private bool HandledStartOfTurnPowers()
        {
            var locationPower = _contexts.TurnPcLocation?.GetStartOfTurnPower();

            var characterPower = _contexts.TurnContext.Character.GetStartOfTurnPower();

            if (characterPower != null && _contexts.TurnContext.PerformedCharacterPowers.Contains(characterPower.Value))
                characterPower = null;

            if (locationPower != null && _contexts.TurnContext.PerformedLocationPowers.Contains(locationPower.Value))
                locationPower = null;

            if (locationPower == null && characterPower == null)
                return false;

            // We'll need to process this again in case there are more valid powers.
            _gameFlow.Interrupt(this);

            GameEvents.SetStatusText("Use Start-of-Turn Power?");

            var resolvable = new PowersAvailableResolvable(locationPower, characterPower, _gameServices)
            {
                HideCancelButton = true
            };
            var processor = new NewResolvableProcessor(resolvable, _gameServices);
            _gameFlow.StartPhase(processor, "Start-of-Turn");

            return true;
        }
    }
}
