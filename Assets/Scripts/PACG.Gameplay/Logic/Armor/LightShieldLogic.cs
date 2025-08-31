using System.Collections.Generic;
using System.Linq;
using PACG.Core;
using UnityEngine;

namespace PACG.Gameplay
{
    public class LightShieldLogic : CardLogicBase
    {
        private readonly ActionStagingManager _asm;
        private readonly ContextManager _contexts;

        private CheckContext Check => _contexts.CheckContext;

        public LightShieldLogic(GameServices gameServices) : base(gameServices) 
        {
            _asm = gameServices.ASM;
            _contexts = gameServices.Contexts;
        }

        public override void OnCommit(IStagedAction action)
        {
            _contexts.EncounterContext?.AddProhibitedTraits(action.Card.Owner, "2-Handed");

            if (action.ActionType != ActionType.Recharge) return;
            
            // TODO: Implement reroll logic.
            Debug.LogWarning($"[{GetType().Name}] Reroll logic not implemented.");
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var modifier = new CheckModifier(card)
            {
                ProhibitedTraits = new[] {"2-Handed"}.ToHashSet()
            };
            
            if (CanReveal(card))
                return new List<IStagedAction>
                {
                    new PlayCardAction(card, ActionType.Reveal, modifier, ("IsFreely", true), ("Damage", 1))
                };
            if (CanRecharge(card))
                return new List<IStagedAction>
                {
                    new PlayCardAction(card, ActionType.Recharge, modifier, ("IsFreely", true))
                };
            
            return new List<IStagedAction>();
        }

        // Can freely reveal once if the owner has a Combat DamageResolvable.
        private bool CanReveal(CardInstance card) =>
            !_asm.CardStaged(card)
            && _contexts.CurrentResolvable is DamageResolvable { DamageType: "Combat" } resolvable
            && resolvable.PlayerCharacter == card.Owner;

        private bool CanRecharge(CardInstance card) => 
            // We can freely recharge to reroll if we're processing a RerollResolvable and the dice pool has a d4, d6, or d8.
            Check != null
            && _contexts.CurrentResolvable is RerollResolvable resolvable
            && card.Owner == resolvable.Character
            && Check.UsedSkill == Skill.Melee
            && resolvable.DicePool.NumDice(4, 6, 8) > 0;
    }
}
