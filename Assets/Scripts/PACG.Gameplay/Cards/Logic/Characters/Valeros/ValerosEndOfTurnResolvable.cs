using PACG.SharedAPI;
using System.Collections.Generic;
using System.Linq;
using PACG.Data;
using UnityEngine;

namespace PACG.Gameplay
{
    public class ValerosEndOfTurnResolvable : BaseResolvable
    {
        private readonly CharacterPower _characterPower;
        private readonly IReadOnlyList<CardInstance> _validCards;

        // Dependency injection
        private readonly ActionStagingManager _asm;
        private readonly ContextManager _contexts;

        public override bool CancelAbortsPhase => true;

        public ValerosEndOfTurnResolvable(List<CardInstance> cards, CharacterPower characterPower, GameServices gameServices)
        {
            _characterPower = characterPower;
            _validCards = cards;

            _asm = gameServices.ASM;
            _contexts = gameServices.Contexts;
        }

        public override List<IStagedAction> GetAdditionalActionsForCard(CardInstance card)
        {
            // Only one card allowed.
            if (_asm.StagedCards.Count > 0)
                return new List<IStagedAction>();

            List<IStagedAction> actions = new();
            if (_validCards.Contains(card))
                actions.Add(new DefaultAction(card, PF.ActionType.Recharge));

            return actions;
        }

        public override bool CanCommit(List<IStagedAction> actions)
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

        public override void Resolve()
        {
            _contexts.TurnContext.PerformedCharacterPowers.Add(_characterPower);
        }
    }
}
