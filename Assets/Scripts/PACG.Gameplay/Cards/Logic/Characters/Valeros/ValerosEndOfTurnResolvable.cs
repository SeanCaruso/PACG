using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PACG.Gameplay
{
    public class ValerosEndOfTurnResolvable : IResolvable
    {
        private readonly IReadOnlyList<CardInstance> _validCards;

        private readonly ActionStagingManager _asm;

        public ValerosEndOfTurnResolvable(List<CardInstance> cards, GameServices gameServices)
        {
            _validCards = cards;

            _asm = gameServices.ASM;
        }

        // No processor necessary.
        public IProcessor CreateProcessor(GameServices gameServices) => null;

        public List<IStagedAction> GetAdditionalActionsForCard(CardInstance card)
        {
            // Only one card allowed.
            if (_asm.StagedCards.Count > 0)
                return new();

            List<IStagedAction> actions = new();
            if (_validCards.Contains(card))
                actions.Add(new DefaultAction(PF.ActionType.Recharge));

            return actions;
        }

        public bool IsResolved(List<IStagedAction> actions) => actions.Count == 1;
    }
}
