using System;
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class LightShieldLogic : CardLogicBase
    {
        private CheckContext Check => GameServices.Contexts.CheckContext;

        private PlayCardAction GetDamageAction(CardInstance card) => new(this, card, PF.ActionType.Reveal, ("IsFreely", true), ("Damage", 1));
        private PlayCardAction GetRerollAction(CardInstance card) => new(this, card, PF.ActionType.Recharge, ("IsFreely", true));

        public LightShieldLogic(GameServices gameServices) : base(gameServices) { }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (CanReveal(card)) actions.Add(GetDamageAction(card));
            if (CanRecharge(card)) actions.Add(GetRerollAction(card));
            return actions;
        }

        bool CanReveal(CardInstance card) => (
            GameServices.Contexts.CurrentResolvable is DamageResolvable resolvable
            && resolvable.DamageType == "Combat"
            && resolvable.PlayerCharacter == card.Owner);

        bool CanRecharge(CardInstance card) => (
            // We can freely recharge to reroll if we're in the dice phase of a Melee combat check and the dice pool has a d4, d6, or d8.
            Check != null
            && Check.Resolvable is CombatResolvable
            && Check.CheckPhase == CheckPhase.RollDice
            && Check.UsedSkill == PF.Skill.Melee
            && Check.DicePool.NumDice(4, 6, 8) > 0);
    }
}
