using PACG.Data;
using PACG.SharedAPI;

namespace PACG.Gameplay
{
    public class PlayerPowerAvailableResolvable : BaseResolvable
    {
        private readonly CharacterPower _power;
        private readonly IResolvable _powerResolvable;

        private readonly GameFlowManager _gameFlow;
        private readonly GameServices _gameServices;

        public override bool CancelAbortsPhase => true;

        public PlayerPowerAvailableResolvable(CharacterPower power, IResolvable powerResolvable, GameServices gameServices)
        {
            _power = power;
            _powerResolvable = powerResolvable;

            _gameFlow = gameServices.GameFlow;
            _gameServices = gameServices;

            GameEvents.RaisePlayerPowerEnabled(power, true, powerResolvable);
        }

        public override void Resolve()
        {
            GameEvents.RaisePlayerPowerEnabled(_power, false);
        }

        public void DoNextResolvable() 
        {
            _gameFlow.StartPhase(new NewResolvableProcessor(_powerResolvable, _gameServices), "ResolvableSequence");
        }
    }
}
