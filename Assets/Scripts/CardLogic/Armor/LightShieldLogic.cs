using System;
using System.Collections.Generic;
using UnityEngine;

[PlayableLogicFor("LightShield")]
public class LightShieldLogic : IPlayableLogic
{
    public CardData CardData { get; set; }

    private PlayCardAction _damageAction;
    private PlayCardAction DamageAction => _damageAction ??= new(this, CardData, PF.ActionType.Reveal, ("IsFreely", true), ("Damage", 1));

    private PlayCardAction _rerollAction;
    private PlayCardAction RerollAction => _rerollAction ??= new(this, CardData, PF.ActionType.Recharge, ("IsFreely", true));

    public List<IStagedAction> GetAvailableCardActions()
    {
        List<IStagedAction> actions = new();
        if (CanReveal()) actions.Add(DamageAction);
        if (CanRecharge()) actions.Add(RerollAction);
        return actions;
    }

    bool CanReveal()
    {
        return (Game.ResolutionContext?.CurrentResolvable is DamageResolvable resolvable
            && resolvable.DamageType == "Combat"
            && resolvable.PlayerCharacter == CardData.Owner);
    }

    bool CanRecharge()
    {
        // We can freely recharge to reroll if we're in the dice phase of a Melee combat check and the dice pool has a d4, d6, or d8.
        return (Game.CheckContext.CheckCategory == CheckCategory.Combat &&
            Game.CheckContext.CheckPhase == CheckPhase.RollDice &&
            Game.CheckContext.UsedSkill == PF.Skill.Melee &&
            Game.CheckContext.DicePool.NumDice(4, 6, 8) > 0
            );
    }

    public void OnStage(IStagedAction _)
    {
    }
    
    public void OnUndo(IStagedAction _)
    {
    }

    public void Execute(IStagedAction action)
    {
        // Damage reduction is handled by DamageResolvable.

        // TODO: Implement reroll die choice.
        if (action == RerollAction) Debug.LogError($"{CardData.cardName} power not implemented!");
    }
}
