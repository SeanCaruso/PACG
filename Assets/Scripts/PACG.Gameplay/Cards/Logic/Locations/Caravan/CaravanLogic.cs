using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class CaravanLogic : LocationLogicBase
    {
        private readonly ContextManager _contexts;

        public CaravanLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        public override List<IResolvable> GetToCloseResolvables()
        {
            int dc = 5 + _contexts.GameContext.AdventureNumber;

            // TODO: Handle when a character who avenges can close.
            return new() { new SkillResolvable(_contexts.TurnContext.Character, dc, PF.Skill.Wisdom, PF.Skill.Perception) };
        }

        // TODO: Implement when we have multiple characters and locations.
        public override List<IResolvable> GetWhenClosedResolvables() => new();
    }
}