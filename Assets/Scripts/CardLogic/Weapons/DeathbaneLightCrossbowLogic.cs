using System;
using System.Collections.Generic;
using UnityEngine;

[PlayableLogicFor("DeathbaneLightCrossbow")]
public class DeathbaneLightCrossbowLogic : IPlayableLogic
{
    public CardData CardData { get; set; }

    private const int RevealIndex = 0;
    private const int DiscardIndex = 1;
    private const int ReloadIndex = 2;

    public List<IStagedAction> GetAvailableActions()
    {
        List<IStagedAction> actions = new();
        if (Game.CheckContext?.CheckCategory == CheckCategory.Combat && Game.CheckContext?.CheckPhase == CheckPhase.PlayCards)
        {
            actions.Add(new PlayCardAction(this, CardData, PF.ActionType.Reveal, powerIndex: RevealIndex, isCombat: true));

            // TODO - Add Distant combat check logic.
        }
        return actions;
    }

    public void OnStage(int? powerIndex = null)
    {
        //context.ResolutionManager.StageAction(new());
    }

    public void ExecuteCardLogic(int? powerIndex = null)
    {
        if (powerIndex == RevealIndex)
        {
            // Reveal to use Dexterity or Ranged + 1d8+1.
            (int die, int bonus) rangedSkill = Game.TurnContext.CurrentPC.GetSkill(PF.Skill.Ranged);
            (int die, int bonus) dexSkill = Game.TurnContext.CurrentPC.GetAttr(PF.Skill.Dexterity);

            var (skill, die, bonus) = rangedSkill.die >= dexSkill.die ? (PF.Skill.Ranged, rangedSkill.die, rangedSkill.bonus) : (PF.Skill.Dexterity, dexSkill.die, dexSkill.bonus);
            Game.CheckContext.UsedSkill = skill;
            Game.CheckContext.DicePool.AddDice(1, die, bonus);
            Game.CheckContext.DicePool.AddDice(1, 8, 1);

            // Against an Undead bane, add another 1d8.
            if (Game.CheckContext.ContextData.TryGetValue("EncounteredCard", out var cardData)
                && cardData is CardData encounteredCard
                && (encounteredCard.traits?.Contains("Undead") ?? false))
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
