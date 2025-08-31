using System.Collections.Generic;
using System.Linq;
using PACG.Core;
using PACG.Data;
using PACG.SharedAPI;

namespace PACG.Gameplay
{
    public class DamageResolvable : BaseResolvable
    {
        public PlayerCharacter PlayerCharacter { get; }
        public string DamageType { get; }
        public int Amount { get; set; }
        private int _currentResolved;

        // Dependency injection
        private readonly ActionStagingManager _asm;

        private ActionType _defaultActionType = ActionType.Discard;
        public void OverrideActionType(ActionType actionType) => _defaultActionType = actionType;

        public DamageResolvable(PlayerCharacter playerCharacter, int amount, GameServices gameServices,
            string damageType = "Combat")
        {
            _asm = gameServices.ASM;

            PlayerCharacter = playerCharacter;
            Amount = amount;
            DamageType = damageType;
        }

        public override List<IStagedAction> GetAdditionalActionsForCard(CardInstance card)
        {
            List<IStagedAction> actions = new();

            if (_currentResolved >= Amount) return actions;

            // Add default damage discard action if the card was in the player's hand.
            if (PlayerCharacter.Hand.Contains(card))
                actions.Add(new DefaultAction(card, _defaultActionType));

            return actions;
        }

        public override bool CanCommit(IReadOnlyList<IStagedAction> actions)
        {
            // If the player's hand size is less than or equal to the damage amount, this can always be resolved by discarding the entire hand.
            //if (PlayerCharacter.hand.Count <= Amount) return true;

            // This was presenting issues, so require manually discarding everything for now.
            if (PlayerCharacter.Hand.Count == 0)
            {
                GameEvents.SetStatusText("");
                return true;
            }

            var totalResolved = 0;
            foreach (var action in actions)
            {
                switch (action)
                {
                    case DefaultAction:
                        totalResolved += 1;
                        break;
                    case PlayCardAction playAction:
                        totalResolved += (int)playAction.ActionData.GetValueOrDefault("Damage", 0);
                        Amount = (int)playAction.ActionData.GetValueOrDefault("ReduceDamageTo", Amount);
                        break;
                }
            }

            _currentResolved = totalResolved;

            if (totalResolved >= Amount)
            {
                GameEvents.SetStatusText("");
                return true;
            }

            GameEvents.SetStatusText($"Damage: Discard {Amount - totalResolved}");

            return false;
        }

        public override bool CanStageAction(IStagedAction action) =>
            action.IsFreely || CanStageType(action.Card.CardType);

        public override bool CanStageType(CardType cardType)
        {
            // Rule: prevent duplicate card types (if not freely playable).
            var stagedActions = _asm.StagedActions;
            return stagedActions.Count(a => a.Card.CardType == cardType && !a.IsFreely) == 0;
        }
    }
}
