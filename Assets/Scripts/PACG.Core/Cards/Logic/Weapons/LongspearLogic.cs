using System;
using System.Collections.Generic;
using System.Linq;

[PlayableLogicFor("Longspear")]
public class LongspearLogic : CardLogicBase, IPlayableLogic
{
    private PlayCardAction _revealAction;
    private PlayCardAction RevealAction => _revealAction ??= new(this, Card, PF.ActionType.Reveal, ("IsCombat", true));

    private PlayCardAction _rerollAction;
    private PlayCardAction RerollAction => _rerollAction ??= new(this, Card, PF.ActionType.Discard, ("IsFreely", true));

    List<IStagedAction> IPlayableLogic.GetAvailableCardActions()
    {
        List<IStagedAction> actions = new();
        if (IsCardPlayable)
        {
            if (Contexts.CheckContext.CheckPhase == CheckPhase.PlayCards
                && !Contexts.CheckContext.StagedCardTypes.Contains(Card.Data.cardType))
            {
                actions.Add(RevealAction);
            }

            // We can discard to reroll if we're in the roll dice phase and this card is one of the reroll options.
            if (Contexts.CheckContext.CheckPhase == CheckPhase.RollDice
                && ((List<CardInstance>)Contexts.CheckContext.ContextData.GetValueOrDefault("rerollCards", new List<CardInstance>())).Contains(Card))
            {
                actions.Add(RerollAction);
            }
        }
        return actions;
    }

    bool IsCardPlayable => (
        // All powers on this card are specific to its owner during a Strength or Melee combat check.
        Contexts.CheckContext != null
        && Contexts.CheckContext.CheckPC == Card.Owner
        && Contexts.CheckContext.CheckCategory == CheckCategory.Combat
        && Contexts.CheckContext.CanPlayCardWithSkills(PF.Skill.Strength, PF.Skill.Melee));

    void IPlayableLogic.OnStage(IStagedAction action)
    {
        Contexts.EncounterContext.AddProhibitedTraits(Card.Owner, Card, "Offhand");
        Contexts.CheckContext.RestrictValidSkills(Card, PF.Skill.Strength, PF.Skill.Melee);
    }

    void IPlayableLogic.OnUndo(IStagedAction action)
    {
        Contexts.EncounterContext.UndoProhibitedTraits(Card.Owner, Card);
        Contexts.CheckContext.UndoSkillModification(Card);
    }

    void IPlayableLogic.Execute(IStagedAction action)
    {
        if (!Contexts.CheckContext.ContextData.ContainsKey("rerollCards"))
            Contexts.CheckContext.ContextData["rerollCards"] = new List<CardData>();
        List<CardInstance> rerollSources = (List<CardInstance>)Contexts.CheckContext.ContextData["rerollCards"];

        // Reveal to use Strength or Melee + 1d8.
        if (action == RevealAction)
        {
            (PF.Skill skill, int die, int bonus) = Card.Owner.GetBestSkill(PF.Skill.Strength, PF.Skill.Melee);
            Contexts.CheckContext.UsedSkill = skill;
            Contexts.CheckContext.DicePool.AddDice(1, die, bonus);
            Contexts.CheckContext.DicePool.AddDice(1, 8);

            rerollSources.Add(Card);
        }

        // Discard to reroll.
        if (action == RerollAction)
        {
            rerollSources.Remove(Card);
            Contexts.CheckContext.ContextData["doReroll"] = true;
        }
    }
}
