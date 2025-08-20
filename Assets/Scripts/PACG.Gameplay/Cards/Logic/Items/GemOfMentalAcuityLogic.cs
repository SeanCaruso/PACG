using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PACG.Gameplay
{
    public class GemOfMentalAcuityLogic : CardLogicBase
    {
        // Dependency injection
        private readonly ContextManager _contexts;
        
        public GemOfMentalAcuityLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        public override void Execute(CardInstance card, IStagedAction action)
        {
            if (_contexts.CheckContext == null) return;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            // Usable on any non-combat check by the owner.
            if (_contexts.CheckContext is { Resolvable: not CombatResolvable } &&
                !_contexts.CheckContext.StagedCardTypes.Contains(PF.CardType.Item) &&
                _contexts.CheckContext.Character == card.Owner)
            {
                return new List<IStagedAction>{ new PlayCardAction(card, PF.ActionType.Recharge) };
            }
            
            return new List<IStagedAction>();
        }
    }
}
