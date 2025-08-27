using PACG.SharedAPI;
using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public class DamageResolvable : BaseResolvable
    {
        public PlayerCharacter PlayerCharacter { get; }
        public string DamageType { get; }
        public int Amount { get; private set; }
        private int _currentResolved;
        
        private PF.ActionType _defaultActionType = PF.ActionType.Discard;
        public void OverrideActionType(PF.ActionType actionType) => _defaultActionType = actionType;

        public DamageResolvable(PlayerCharacter playerCharacter, int amount, string damageType = "Combat")
        {
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

        public override bool CanCommit(List<IStagedAction> actions)
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
    }
}
