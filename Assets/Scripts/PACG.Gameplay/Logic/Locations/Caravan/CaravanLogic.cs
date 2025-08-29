
using PACG.Core;

namespace PACG.Gameplay
{
    public class CaravanLogic : LocationLogicBase
    {
        private readonly ContextManager _contexts;

        public CaravanLogic(GameServices gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        public override IResolvable GetToCloseResolvable()
        {
            var dc = 5 + _contexts.GameContext.AdventureNumber;

            // TODO: Handle when a character who avenges can close.
            return new CheckResolvable(
                _contexts.TurnPcLocation,
                _contexts.TurnContext.Character,
                CardUtils.SkillCheck(dc,Skill.Wisdom, Skill.Perception)
            );
        }

        // TODO: Implement when we have multiple characters and locations.
        public override IResolvable GetWhenClosedResolvable() => null;
    }
}
