using System;
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    [PlayableLogicFor("LightShield")]
    public class LightShieldLogic : CardLogicBase, IPlayableLogic
    {
        private CheckContext Check => GameServices.Contexts.CheckContext;

        private PlayCardAction _damageAction;
        private PlayCardAction DamageAction => _damageAction ??= new(this, Card, PF.ActionType.Reveal, ("IsFreely", true), ("Damage", 1));

        private PlayCardAction _rerollAction;
        private PlayCardAction RerollAction => _rerollAction ??= new(this, Card, PF.ActionType.Recharge, ("IsFreely", true));

        public LightShieldLogic(GameServices gameServices) : base(gameServices) { }

        protected override List<IStagedAction> GetAvailableCardActions()
        {
            List<IStagedAction> actions = new();
            if (CanReveal) actions.Add(DamageAction);
            if (CanRecharge) actions.Add(RerollAction);
            return actions;
        }

        bool CanReveal => (
            GameServices.Contexts.ResolutionContext?.CurrentResolvable is DamageResolvable resolvable
            && resolvable.DamageType == "Combat"
            && resolvable.PlayerCharacter == Card.Owner);

        bool CanRecharge => (
            // We can freely recharge to reroll if we're in the dice phase of a Melee combat check and the dice pool has a d4, d6, or d8.
            Check != null
            && Check.Resolvable is CombatResolvable
            && Check.CheckPhase == CheckPhase.RollDice
            && Check.UsedSkill == PF.Skill.Melee
            && Check.DicePool.NumDice(4, 6, 8) > 0);

        void IPlayableLogic.OnStage(IStagedAction action) { }

        void IPlayableLogic.OnUndo(IStagedAction action) { }

        void IPlayableLogic.Execute(IStagedAction action)
        {
            // TODO: Implement reroll dice execution.
        }
    }
}
