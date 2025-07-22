using System;
using System.Collections.Generic;
using UnityEngine;

[PlayableLogicFor("DeathbaneLightCrossbow")]
public class DeathbaneLightCrossbowLogic : IPlayableLogic
{
    private const int RevealIndex = 0;
    private const int DiscardIndex = 1;
    private const int ReloadIndex = 2;

    public List<PlayCardAction> GetAvailableActions(ActionContext context, CardData thisCardData)
    {
        List<PlayCardAction> actions = new();
        if (context.CheckCategory == CheckCategory.Combat)
        {
            actions.Add(new(this, thisCardData, PF.ActionType.Reveal, powerIndex: RevealIndex));

            // TODO - Add Distant combat check logic.
        }
        return actions;
    }

    public void OnStage(ActionContext context, int? powerIndex = null)
    {
        //context.ResolutionManager.StageAction(new());
    }

    public void Execute(ActionContext context, int? powerIndex = null)
    {
        if (powerIndex == RevealIndex)
        {
            // Reveal to use Dexterity or Ranged + 1d8+1.
            (int die, int bonus) rangedSkill = context.ActiveCharacter.GetSkill(PF.Skill.Ranged);
            (int die, int bonus) dexSkill = context.ActiveCharacter.GetAttr(PF.Skill.Dexterity);

            var (skill, die, bonus) = rangedSkill.die >= dexSkill.die ? (PF.Skill.Ranged, rangedSkill.die, rangedSkill.bonus) : (PF.Skill.Dexterity, dexSkill.die, dexSkill.bonus);
            context.UsedSkill = skill;
            context.DicePool.AddDice(1, die, bonus);
            context.DicePool.AddDice(1, 8, 1);

            // Against an Undead bane, add another 1d8.
            if (context.ContextData.TryGetValue("EncounteredCard", out var cardData)
                && cardData is CardData encounteredCard
                && (encounteredCard.traits?.Contains("Undead") ?? false))
            {
                context.DicePool.AddDice(1, 8);
            }

        }
        else if ( powerIndex == DiscardIndex || powerIndex == ReloadIndex )
        {
            throw new NotImplementedException();
        }
    }
}
