using System;
using System.Collections.Generic;
using UnityEngine;

[PlayableLogicFor("Longsword")]
public class LongswordLogic : BasePlayableLogic
{
    private const int RevealIndex = 0;
    private const int ReloadIndex = 1;

    public override List<PlayCardAction> GetAvailableActions(ActionContext context)
    {
        List<PlayCardAction> actions = new();
        if (context.CheckCategory == CheckCategory.Combat)
        {
            actions.Add(new(this, CardData, PF.ActionType.Reveal, powerIndex: RevealIndex));

            if (context.TurnContext.CurrentPC.IsProficient(PF.CardType.Weapon))
            {
                actions.Add(new(this, CardData, PF.ActionType.Reload, powerIndex: ReloadIndex));
            }
        }
        return actions;
    }

    public override void OnStage(ActionContext context, int? powerIndex = null)
    {
        //context.ResolutionManager.StageAction(new());
    }

    public override void ExecuteCardLogic(ActionContext context, int? powerIndex = null)
    {
        if (powerIndex is not null)
        {
            // Reveal to use Strength or Melee + 1d8.
            (int die, int bonus) meleeSkill = context.TurnContext.CurrentPC.GetSkill(PF.Skill.Melee);
            (int die, int bonus) strSkill = context.TurnContext.CurrentPC.GetAttr(PF.Skill.Strength);

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
