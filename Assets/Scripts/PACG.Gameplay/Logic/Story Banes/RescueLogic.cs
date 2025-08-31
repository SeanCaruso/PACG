using System.Collections.Generic;
using PACG.Core;
using PACG.Data;
using UnityEngine;

namespace PACG.Gameplay
{
    public class RescueLogic : CardLogicBase
    {
        // Dependency injection
        private readonly ContextManager _contexts;
        
        public RescueLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();
            
            // Freely recharge an ally for +1d4.
            if (card.CardType != CardType.Ally
                || _contexts.CheckContext?.Character != card.Owner) return actions;
            
            var modifier = new CheckModifier(card) { AddedDice = new List<int> { 4 } };
            actions.Add(new PlayCardAction(card, ActionType.Recharge, modifier, ("IsFreely", true)));

            return actions;
        }

        public override void OnDefeated(CardInstance card)
        {
            // TODO: Draw a new ally that lists Diplomacy in its check to acquire from the Vault.
            Debug.LogWarning("Rescue on defeat logic not implemented yet!");
            base.OnDefeated(card);
        }
    }
}
