using System;
using System.Collections.Generic;
using UnityEngine;

[PlayableLogicFor("Longsword")]
public class LongswordLogic : IPlayableLogic
{
    private const int RevealIndex = 0;
    private const int ReloadIndex = 1;

    public List<PlayCardAction> GetAvailableActions(ActionContext context, CardData thisCardData)
    {
        List<PlayCardAction> actions = new();
        if (context.CheckCategory == CheckCategory.Combat)
        {
            actions.Add(new(this, thisCardData, PF.ActionType.Reveal, powerIndex: RevealIndex));

            if (context.ActiveCharacter.IsProficient(PF.CardType.Weapon))
            {
                actions.Add(new(this, thisCardData, PF.ActionType.Reload, powerIndex: ReloadIndex));
            }
        }
        return actions;
    }

    public void OnStage(ActionContext context, int? powerIndex = null)
    {
        //context.ResolutionManager.StageAction(new());
    }

    public void Execute(ActionContext context, int? powerIndex = null)
    {
        if (powerIndex is not null)
        {
            // Reveal to use Strength or Melee + 1d8.
            (int die, int bonus) meleeSkill = context.ActiveCharacter.GetSkill(PF.Skill.Melee);
            (int die, int bonus) strSkill = context.ActiveCharacter.GetAttr(PF.Skill.Strength);

            var (skill, die, bonus) = meleeSkill.die >= strSkill.die ? (PF.Skill.Melee, meleeSkill.die, meleeSkill.bonus) : (PF.Skill.Strength, strSkill.die, strSkill.bonus);
            context.UsedSkill = skill;
            context.DicePool.AddDice(1, die, bonus);
            context.DicePool.AddDice(1, 8);

            // Reload to add another 1d4.
            if (powerIndex == ReloadIndex)
            {
                context.DicePool.AddDice(1, 4);
            }
        }
    }
}
