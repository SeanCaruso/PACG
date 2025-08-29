using System.Collections.Generic;
using PACG.Data;
using PACG.SharedAPI;

namespace PACG.Gameplay
{
    public class PowersAvailableResolvable : BaseResolvable
    {
        private readonly LocationPower? _locationPower;
        private readonly CharacterPower? _characterPower;
        
        // Dependency injection
        private readonly ContextManager _contexts;

        public override bool CancelAbortsPhase => true;
        public bool HideCancelButton { get; set; }

        public PowersAvailableResolvable(LocationPower? locationPower,
            CharacterPower? characterPower,
            GameServices gameServices)
        {
            _locationPower = locationPower;
            _characterPower = characterPower;
            
            _contexts = gameServices.Contexts;

            if (_locationPower != null)
                GameEvents.RaiseLocationPowerEnabled(_locationPower.Value, true);
            if (_characterPower != null)
                GameEvents.RaisePlayerPowerEnabled(_characterPower.Value, true);
        }

        public override void Resolve()
        {
            if (_locationPower != null)
                GameEvents.RaiseLocationPowerEnabled(_locationPower.Value, false);
            if (_characterPower != null)
                GameEvents.RaisePlayerPowerEnabled(_characterPower.Value, false);
        }

        public override void OnSkip()
        {
            if (_locationPower != null)
                _contexts.TurnContext.PerformedLocationPowers.Add(_locationPower.Value);
            if (_characterPower != null)
                _contexts.TurnContext.PerformedCharacterPowers.Add(_characterPower.Value);
        }

        public override StagedActionsState GetUIState(IReadOnlyList<IStagedAction> actions)
        {
            var baseState = base.GetUIState(actions);
            if (HideCancelButton)
                baseState.IsCancelButtonVisible = false;
            return baseState;
        }
    }
}
