using PACG.SharedAPI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PACG.Gameplay
{
    public class ValerosEndOfTurnResolvable : BaseResolvable
    {
        private readonly IReadOnlyList<CardInstance> _validCards;

        private readonly ActionStagingManager _asm;

        public ValerosEndOfTurnResolvable(List<CardInstance> cards, GameServices gameServices)
        {
            _validCards = cards;

            _asm = gameServices.ASM;
        }

        public override List<IStagedAction> GetAdditionalActionsForCard(CardInstance card)
        {
            // Only one card allowed.
            if (_asm.StagedCards.Count > 0)
                return new();

            List<IStagedAction> actions = new();
            if (_validCards.Contains(card))
                actions.Add(new DefaultAction(card, PF.ActionType.Recharge));

            return actions;
        }

        public override bool IsResolved(List<IStagedAction> actions)
        {
            if (actions.Count == 1)
            {
                GameEvents.SetStatusText("");
                return true;
            }
            else
            {
                GameEvents.SetStatusText("Recharge a weapon or an armor from your hand or discards.");
                return false;
            }
        }
    }
}
