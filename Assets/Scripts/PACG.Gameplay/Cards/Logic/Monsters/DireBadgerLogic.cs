
using PACG.Core;
using UnityEngine;

namespace PACG.Gameplay
{
    public class DireBadgerLogic : CardLogicBase
    {
        // Dependency injections
        private readonly ContextManager _contexts;
        
        public DireBadgerLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        public override IResolvable GetResolveEncounterResolvable(CardInstance card)
        {
            var result = _contexts.EncounterContext.CheckResult;
            
            if (result.WasSuccess && result.IsCombat)
            {
                return new DamageResolvable(_contexts.EncounterContext.Character, DiceUtils.Roll(4));
            }

            return null;
        }

        public override void OnUndefeated(CardInstance card)
        {
            // If undefeated, shuffle into a random location.
            var locations = _contexts.GameContext.Locations;
            var newLocation = locations[DiceUtils.Roll(locations.Count) - 1];
            newLocation.ShuffleIn(card, true);
            
            Debug.Log($"[{GetType().Name}] {card} shuffled into {newLocation}.");
        }
    }
}
