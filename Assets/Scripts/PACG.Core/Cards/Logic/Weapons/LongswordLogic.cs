using System;
using System.Collections.Generic;
using System.Linq;

[PlayableLogicFor("Longsword")]
public class LongswordLogic : CardLogicBase, IPlayableLogic
{
    private PlayCardAction _revealAction;
    private PlayCardAction RevealAction => _revealAction ??= new(this, Card, PF.ActionType.Reveal, ("IsCombat", true));

    private PlayCardAction _reloadAction;
    private PlayCardAction ReloadAction => _reloadAction ??= new(this, Card, PF.ActionType.Reload, ("IsCombat", true), ("IsFreely", true));

    private PlayCardAction _revealAndReloadAction;
    private PlayCardAction RevealAndReloadAction => _revealAndReloadAction ??= new(this, Card, PF.ActionType.Reload, ("IsCombat", true));

    List<IStagedAction> IPlayableLogic.GetAvailableCardActions()
    {
        List<IStagedAction> actions = new();
        if (IsCardPlayabe)
        {
            // If a weapon hasn't been played yet, present one or both options.
            if (!Contexts.CheckContext.StagedCardTypes.Contains(Card.Data.cardType))
            {
                actions.Add(RevealAction);

                if (Contexts.CheckContext.CheckPC.IsProficient(PF.CardType.Weapon))
                {
                    actions.Add(RevealAndReloadAction);
                }
            }
            // Otherwise, if this card has already been played, present the reload option if proficient.
            else if (Contexts.CheckContext.StagedCards.Contains(Card) && Contexts.CheckContext.CheckPC.IsProficient(PF.CardType.Weapon))
            {
                actions.Add(ReloadAction);
            }
        }
        return actions;
    }

    bool IsCardPlayabe => (
        // All powers are specific to the card's owner while playing cards during a Strength or Melee combat check.
        Contexts.CheckContext != null
        && Contexts.CheckContext.CheckPC == Card.Owner
        && Contexts.CheckContext.CheckCategory == CheckCategory.Combat
        && Contexts.CheckContext.CheckPhase == CheckPhase.PlayCards
        && Contexts.CheckContext.CanPlayCardWithSkills(PF.Skill.Strength, PF.Skill.Melee));

    void IPlayableLogic.OnStage(IStagedAction action)
    {
        Contexts.CheckContext.RestrictValidSkills(Card, PF.Skill.Strength, PF.Skill.Melee);
    }
    
    void IPlayableLogic.OnUndo(IStagedAction action)
    {
        Contexts.CheckContext.UndoSkillModification(Card);
    }

    void IPlayableLogic.Execute(IStagedAction action)
    {
        if (action == RevealAction || action == RevealAndReloadAction)
        {
            // Reveal to use Strength or Melee + 1d8.
            (PF.Skill skill, int die, int bonus) = Contexts.CheckContext.CheckPC.GetBestSkill(PF.Skill.Strength, PF.Skill.Melee);
            Contexts.CheckContext.UsedSkill = skill;
            Contexts.CheckContext.DicePool.AddDice(1, die, bonus);
            Contexts.CheckContext.DicePool.AddDice(1, 8);
        }

        // Reload to add another 1d4.
        if (action == ReloadAction || action == RevealAndReloadAction)
        {
            Contexts.CheckContext.DicePool.AddDice(1, 4);
        }
    }
}
