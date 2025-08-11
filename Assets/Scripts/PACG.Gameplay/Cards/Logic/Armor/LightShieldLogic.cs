using System;
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class LightShieldLogic : CardLogicBase
    {
        private readonly ActionStagingManager _asm;
        private readonly CheckContext _check;
        private readonly ContextManager _contexts;

        private PlayCardAction GetDamageAction(CardInstance card) => new(card, PF.ActionType.Reveal, ("IsFreely", true), ("Damage", 1));
        private PlayCardAction GetRerollAction(CardInstance card) => new(card, PF.ActionType.Recharge, ("IsFreely", true));

        public LightShieldLogic(GameServices gameServices) : base(gameServices) 
        {
            _asm = gameServices.ASM;
            _contexts = gameServices.Contexts;

            _check = _contexts.CheckContext;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (CanReveal(card)) actions.Add(GetDamageAction(card));
            if (CanRecharge(card)) actions.Add(GetRerollAction(card));
            return actions;
        }

        // Can freely reveal once if the owner has a Combat DamageResolvable.
        bool CanReveal(CardInstance card) => (
            !_asm.CardStaged(card)
            && _contexts.CurrentResolvable is DamageResolvable resolvable
            && resolvable.DamageType == "Combat"
            && resolvable.PlayerCharacter == card.Owner);

        bool CanRecharge(CardInstance card) => (
            // We can freely recharge to reroll if we're in the dice phase of a Melee combat check and the dice pool has a d4, d6, or d8.
            _check != null
            && _check.Resolvable is CombatResolvable
            && _check.CheckPhase == CheckPhase.RollDice
            && _check.UsedSkill == PF.Skill.Melee
            && _check.DicePool.NumDice(4, 6, 8) > 0);
    }
}
