using System;
using System.Collections.Generic;
using UnityEngine;

/*
[PlayableLogicFor("DeathbaneLightCrossbow")]
public class DeathbaneLightCrossbowLogic : IPlayableLogic
{
    public CardData CardData { get; set; }

    private const int RevealIndex = 0;
    private const int DiscardIndex = 1;
    private const int ReloadIndex = 2;

    public List<IStagedAction> GetAvailableCardActions()
    {
        List<IStagedAction> actions = new();
        if (!Game.CheckContext.StagedCardTypes.Contains(CardData.cardType))
        {
            if (Game.CheckContext.CheckPC == CardData.Owner
                && Game.CheckContext.CheckCategory == CheckCategory.Combat
                && Game.CheckContext.CheckPhase == CheckPhase.PlayCards)
            {
                actions.Add(new PlayCardAction(this, CardData, PF.ActionType.Reveal, powerIndex: RevealIndex, isCombatCheck: true));

                // TODO - Add Distant combat check logic. (And update this entire implementation to match standards.)
            }
        }
        return actions;
    }

    public void OnStage(int? powerIndex = null)
    {
        Game.Stage(CardData);
    }

    public void OnUndo(int? powerIndex = null)
    {
        Game.Undo(CardData);
    }

    public void ExecuteCardLogic(int? powerIndex = null)
    {
        if (powerIndex == RevealIndex)
        {
            // Reveal to use Dexterity or Ranged + 1d8+1.
            (PF.Skill skill, int die, int bonus) = CardData.Owner.GetBestSkill(PF.Skill.Dexterity, PF.Skill.Ranged);
            Game.CheckContext.UsedSkill = skill;
            Game.CheckContext.DicePool.AddDice(1, die, bonus);
            Game.CheckContext.DicePool.AddDice(1, 8, 1);

            // Against an Undead bane, add another 1d8.
            if (Game.EncounterContext.EncounteredCardData.traits?.Contains("Undead") ?? false)
            {
                Game.CheckContext.DicePool.AddDice(1, 8);
            }

        }
        else if ( powerIndex == DiscardIndex || powerIndex == ReloadIndex )
        {
            throw new NotImplementedException();
        }
    }
}
*/