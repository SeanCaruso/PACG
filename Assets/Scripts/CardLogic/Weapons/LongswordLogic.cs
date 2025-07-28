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
        List<IStagedAction> actions = new();
        if (Game.CheckContext?.CheckCategory == CheckCategory.Combat && Game.CheckContext.CheckPhase == CheckPhase.PlayCards)
        {
            actions.Add(new PlayCardAction(this, CardData, PF.ActionType.Reveal, powerIndex: RevealIndex, isCombat: true));

            if (Game.TurnContext.CurrentPC.IsProficient(PF.CardType.Weapon))
            {
                actions.Add(new PlayCardAction(this, CardData, PF.ActionType.Reload, powerIndex: ReloadIndex, isCombat: true));
            }
        }
        return actions;
    }

    public void OnStage(int? powerIndex = null)
    {
        //context.ResolutionManager.StageAction(new());
    }

    public void ExecuteCardLogic(int? powerIndex = null)
    {
        if (powerIndex is not null)
        {
            // Reveal to use Strength or Melee + 1d8.
            (int die, int bonus) meleeSkill = Game.TurnContext.CurrentPC.GetSkill(PF.Skill.Melee);
            (int die, int bonus) strSkill = Game.TurnContext.CurrentPC.GetAttr(PF.Skill.Strength);

            var (skill, die, bonus) = meleeSkill.die >= strSkill.die ? (PF.Skill.Melee, meleeSkill.die, meleeSkill.bonus) : (PF.Skill.Strength, strSkill.die, strSkill.bonus);
            Game.CheckContext.UsedSkill = skill;
            Game.CheckContext.DicePool.AddDice(1, die, bonus);
            Game.CheckContext.DicePool.AddDice(1, 8);

            // Reload to add another 1d4.
            if (powerIndex == ReloadIndex)
            {
                Game.CheckContext.DicePool.AddDice(1, 4);
            }
        }
    }
}
