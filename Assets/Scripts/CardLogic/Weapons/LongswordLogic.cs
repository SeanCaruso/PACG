using System;
using System.Collections.Generic;
using UnityEngine;

[PlayableLogicFor("Longsword")]
public class LongswordLogic : IPlayableLogic
{
    public CardInstance Card { get; set; }

    private PlayCardAction _revealAction;
    private PlayCardAction RevealAction => _revealAction ??= new(this, Card, PF.ActionType.Reveal, ("IsCombat", true));

    private PlayCardAction _reloadAction;
    private PlayCardAction ReloadAction => _reloadAction ??= new(this, Card, PF.ActionType.Reload, ("IsCombat", true), ("IsFreely", true));

    private PlayCardAction _revealAndReloadAction;
    private PlayCardAction RevealAndReloadAction => _revealAndReloadAction ??= new(this, Card, PF.ActionType.Reload, ("IsCombat", true));

    public List<IStagedAction> GetAvailableCardActions()
    {
        List<IStagedAction> actions = new();
        if (IsCardPlayabe)
        {
            // If a weapon hasn't been played yet, present one or both options.
            if (!Game.CheckContext.StagedCardTypes.Contains(Card.Data.cardType))
            {
                actions.Add(RevealAction);

                if (Game.CheckContext.CheckPC.IsProficient(PF.CardType.Weapon))
                {
                    actions.Add(RevealAndReloadAction);
                }
            }
            // Otherwise, if this card has already been played, present the reload option if proficient.
            else if (Game.CheckContext.StagedCards.Contains(Card) && Game.CheckContext.CheckPC.IsProficient(PF.CardType.Weapon))
            {
                actions.Add(ReloadAction);
            }
        }
        return actions;
    }

    bool IsCardPlayabe => (
        // All powers are specific to the card's owner while playing cards during a Strength or Melee combat check.
        Game.CheckContext != null
        && Game.CheckContext.CheckPC == Card.Owner
        && Game.CheckContext.CheckCategory == CheckCategory.Combat
        && Game.CheckContext.CheckPhase == CheckPhase.PlayCards
        && Game.CheckContext.CanPlayCardWithSkills(PF.Skill.Strength, PF.Skill.Melee));

    public void OnStage(IStagedAction action)
    {
        Game.CheckContext.RestrictValidSkills(Card, PF.Skill.Strength, PF.Skill.Melee);
    }
    
    public void OnUndo(IStagedAction action)
    {
        Game.CheckContext.UndoSkillModification(Card);
    }

    public void Execute(IStagedAction action)
    {
        if (action == RevealAction || action == RevealAndReloadAction)
        {
            // Reveal to use Strength or Melee + 1d8.
            (PF.Skill skill, int die, int bonus) = Game.CheckContext.CheckPC.GetBestSkill(PF.Skill.Strength, PF.Skill.Melee);
            Game.CheckContext.UsedSkill = skill;
            Game.CheckContext.DicePool.AddDice(1, die, bonus);
            Game.CheckContext.DicePool.AddDice(1, 8);
        }

        // Reload to add another 1d4.
        if (action == ReloadAction || action == RevealAndReloadAction)
        {
            Game.CheckContext.DicePool.AddDice(1, 4);
        }
    }
}
