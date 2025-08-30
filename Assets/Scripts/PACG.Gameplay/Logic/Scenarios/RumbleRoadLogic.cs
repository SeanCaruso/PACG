using System.Linq;
using PACG.Core;
using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    public class RumbleRoadLogic : ScenarioLogicBase
    {
        // Dependency injections
        private readonly ContextManager _contexts;

        // We have an available turn action if the player has discarded cards or an applicable scourge and can freely
        // explore.
        public override bool HasAvailableAction =>
            (_contexts.TurnContext?.Character.Discards.Count > 0
             || _contexts.TurnContext?.Character.ActiveScourges.Any(s =>
                 s is ScourgeType.Poisoned or ScourgeType.Wounded) == true)
            && _contexts.TurnContext.CanFreelyExplore
            && _contexts.CurrentResolvable == null
            && _contexts.CheckContext == null;

        public RumbleRoadLogic(GameServices gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        public override void InvokeAction()
        {
            // Instead of the first exploration, heal 1d4 cards.
            var pc = _contexts.TurnContext.Character;
            var amount = DiceUtils.Roll(4);
            Debug.Log($"[{GetType().Name}] Healing {pc.Name} for {amount}.");
            pc.Heal(amount);

            _contexts.TurnContext.CanGive = false;
            _contexts.TurnContext.CanMove = false;
            _contexts.TurnContext.CanFreelyExplore = false;
            GameEvents.RaiseTurnStateChanged();
        }
    }
}
