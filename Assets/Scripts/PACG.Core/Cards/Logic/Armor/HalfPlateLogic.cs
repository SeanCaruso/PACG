using System;
using System.Collections.Generic;
using System.Linq;

[PlayableLogicFor("HalfPlate")]
public class HalfPlateLogic : CardLogicBase, IPlayableLogic
{
    private PlayCardAction _displayAction;
    private PlayCardAction DisplayAction => _displayAction ??= new(this, Card, PF.ActionType.Display);

    private PlayCardAction _drawAction;
    private PlayCardAction DrawAction => _drawAction ??= new(this, Card, PF.ActionType.Draw, ("Damage", 2));

    private PlayCardAction _buryAction;
    private PlayCardAction BuryAction => _buryAction ??= new(this, Card, PF.ActionType.Bury, ("ReduceDamageTo", 0));

    List<IStagedAction> IPlayableLogic.GetAvailableCardActions()
    {
        List<IStagedAction> actions = new();
        if (CanDisplay) actions.Add(DisplayAction);
        if (CanDraw) actions.Add(DrawAction);
        if (CanBury) actions.Add(BuryAction);
        return actions;
    }

    bool CanDisplay => (
        // We can display if not currently displayed and we haven't played an Armor during a check.
        !Card.Owner.DisplayedCards.Contains(Card) 
        && (Contexts.CheckContext == null || !Contexts.CheckContext.StagedCardTypes.Contains(PF.CardType.Armor)));

    bool CanDraw => (
        // We can draw for damage if displayed and we have a DamageResolvable for the card's owner with Combat damage.
        Contexts.CheckContext != null
        && Card.Owner.DisplayedCards.Contains(Card)
        && (Contexts.CheckContext.StagedCards.Contains(Card) || !Contexts.CheckContext.StagedCardTypes.Contains(PF.CardType.Armor)) // If we staged the Display this check, we can freely draw.
        && Contexts.ResolutionContext?.CurrentResolvable is DamageResolvable resolvable
        && resolvable.DamageType == "Combat" 
        && resolvable.PlayerCharacter == Card.Owner);

    bool CanBury => (
        // We can bury for damage if displayed, the owner is proficient, and we have a DamageResolvable for the card's owner.
        Contexts.CheckContext != null
        && Card.Owner.DisplayedCards.Contains(Card)
        && (Contexts.CheckContext.StagedCards.Contains(Card) || !Contexts.CheckContext.StagedCardTypes.Contains(PF.CardType.Armor)) // If we staged the Display this check, we can freely bury.
        && Card.Owner.IsProficient(PF.CardType.Armor)
        && Contexts.ResolutionContext?.CurrentResolvable is DamageResolvable resolvable
        && resolvable.PlayerCharacter == Card.Owner);

    void IPlayableLogic.OnStage(IStagedAction action) { }

    void IPlayableLogic.OnUndo(IStagedAction action) { }

    void IPlayableLogic.Execute(IStagedAction action) { }
}
