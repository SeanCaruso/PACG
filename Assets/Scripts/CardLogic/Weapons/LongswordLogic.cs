using System;
using System.Collections.Generic;
using UnityEngine;

[PlayableLogicFor("Longsword")]
public class LongswordLogic : IPlayableLogic
{
    public CardData CardData { get; set; }

    private const int RevealIndex = 0;
    private const int ReloadIndex = 1;

    public List<IStagedAction> GetAvailableCardActions()
    {
        hasExecutedReveal = false;
        List<IStagedAction> actions = new();
        if (IsCardPlayabe)
        {
            PlayCardAction reloadAction = new(this, CardData, PF.ActionType.Reload, powerIndex: ReloadIndex, isCombat: true);

            // If a weapon hasn't been played yet, present one or both options.
            if (!Game.CheckContext.StagedCardTypes.Contains(CardData.cardType))
            {
                actions.Add(new PlayCardAction(this, CardData, PF.ActionType.Reveal, powerIndex: RevealIndex, isCombat: true));

                if (Game.CheckContext.CheckPC.IsProficient(PF.CardType.Weapon))
                {
                    actions.Add(reloadAction);
                }
            }
            // Otherwise, if this card has already been played, present the reload option if proficient.
            else if (Game.CheckContext.StagedCards.Contains(CardData) && Game.CheckContext.CheckPC.IsProficient(PF.CardType.Weapon))
            {
                actions.Add(reloadAction);
            }
        }
        return actions;
    }

    bool IsCardPlayabe => (
        // All powers are specific to the card's owner while playing cards during a combat check.
        Game.CheckContext.CheckPC == CardData.Owner &&
        Game.CheckContext.CheckCategory == CheckCategory.Combat &&
        Game.CheckContext.CheckPhase == CheckPhase.PlayCards
        );

    public void OnStage(int? powerIndex = null)
    {
        Game.Stage(CardData);
    }
    
    public void OnUndo(int? powerIndex = null)
    {
        Game.Undo(CardData);
    }

    bool hasExecutedReveal = false;
    public void ExecuteCardLogic(int? powerIndex = null)
    {
        if (powerIndex is not null)
        {
            if (!hasExecutedReveal)
            {
                // Reveal to use Strength or Melee + 1d8.
                (PF.Skill skill, int die, int bonus) = Game.CheckContext.CheckPC.GetBestSkill(PF.Skill.Strength, PF.Skill.Melee);
                Game.CheckContext.UsedSkill = skill;
                Game.CheckContext.DicePool.AddDice(1, die, bonus);
                Game.CheckContext.DicePool.AddDice(1, 8);

                hasExecutedReveal = true;
            }

            // Reload to add another 1d4.
            if (powerIndex == ReloadIndex)
            {
                Game.CheckContext.DicePool.AddDice(1, 4);
            }
        }
    }
}
