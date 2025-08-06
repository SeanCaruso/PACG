
using System.Collections.Generic;

namespace PACG.Gameplay
{
    public class DamageResolvable : IResolvable
    {
        public LogicRegistry LogicRegistry { get; }
        public PlayerCharacter PlayerCharacter { get; }
        public string DamageType { get; }
        public int Amount { get; protected set; }

        public DamageResolvable(LogicRegistry logicRegistry, PlayerCharacter playerCharacter, int amount, string damageType = "Combat")
        {
            LogicRegistry = logicRegistry;
            PlayerCharacter = playerCharacter;
            Amount = amount;
            DamageType = damageType;
        }

        public List<IStagedAction> GetValidActions()
        {
            List<IStagedAction> actions = new();

            foreach (var card in PlayerCharacter.Hand)
            {
                actions.AddRange(GetValidActionsForCard(card));
            }

            return actions;
        }

        public List<IStagedAction> GetValidActionsForCard(CardInstance card)
        {
            // Grab any card-specific options for handling damage.
            var cardLogic = LogicRegistry.GetPlayableLogic(card);
            List<IStagedAction> actions = cardLogic?.GetAvailableActions() ?? new();

            // Add default damage discard action if the card was in the player's hand.
            if (PlayerCharacter.Hand.Contains(card))
                actions.Add(new DefaultDamageAction(card));

            return actions;
        }

        public bool IsResolved(List<IStagedAction> actions)
        {
            // If the player's hand size is less than or equal to the damage amount, this can always be resolved by discarding the entire hand.
            //if (PlayerCharacter.hand.Count <= Amount) return true;

            // This was presenting issues, so require manually discarding everything for now.
            if (PlayerCharacter.Hand.Count == 0) return true;

            int totalResolved = 0;
            foreach (var action in actions)
            {
                if (action is DefaultDamageAction)
                    totalResolved += 1;
                else if (action is PlayCardAction playAction)
                {
                    totalResolved += (int)playAction.ActionData.GetValueOrDefault("Damage", 0);
                    Amount = (int)playAction.ActionData.GetValueOrDefault("ReduceDamageTo", Amount);
                }
            }

            return totalResolved >= Amount;
        }
    }
}
