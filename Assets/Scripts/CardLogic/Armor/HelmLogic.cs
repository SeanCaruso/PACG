using System;
using System.Collections.Generic;
using UnityEngine;

[PlayableLogicFor("Helm")]
public class HelmLogic : IPlayableLogic
{
    public CardInstance Card { get; set; }

    private PlayCardAction _damageAction;
    private PlayCardAction DamageAction => _damageAction ??= new(this, Card, PF.ActionType.Reveal, ("IsFreely", true), ("Damage", 1));

    public List<IStagedAction> GetAvailableCardActions()
    {
        List<IStagedAction> actions = new();
        if (CanReveal) actions.Add(DamageAction);
        return actions;
    }

    bool CanReveal => (
        // We can freely reveal for damage if we have a DamageResolvable for the card's owner with Combat damage, or any type of damage if proficient.
        Game.ResolutionContext?.CurrentResolvable is DamageResolvable resolvable
        && (resolvable.DamageType == "Combat" || Card.Owner.IsProficient(PF.CardType.Armor))
        && resolvable.PlayerCharacter == Card.Owner);

    public void OnStage(IStagedAction _)
    {
        Game.EncounterContext.AddProhibitedTraits(Card.Owner, Card, "Helm");
    }
    
    public void OnUndo(IStagedAction _)
    {
        Game.EncounterContext.UndoProhibitedTraits(Card.Owner, Card);
    }

    public void Execute(IStagedAction _)
    {
        // Damage reduction is handled by DamageResolvable.
    }
}
