using System;
using System.Collections.Generic;
using UnityEngine;

[PlayableLogicFor("Longspear")]
public class LongspearLogic : IPlayableLogic
{
    public CardData CardData { get; set; }

    private const int RevealIndex = 0;
    private const int DiscardIndex = 1;

    public List<IStagedAction> GetAvailableCardActions()
    {
        List<IStagedAction> actions = new();
        if (Game.CheckContext?.CheckCategory == CheckCategory.Combat)
        {
            if (Game.CheckContext.CheckPhase == CheckPhase.PlayCards)
                actions.Add(new PlayCardAction(this, CardData, PF.ActionType.Reveal, powerIndex: RevealIndex, isCombat: true));

            // We can discard to reroll if we're in the roll dice phase and this card is one of the reroll options.
            if (Game.CheckContext.CheckPhase == CheckPhase.RollDice
                && ((List<CardData>)Game.CheckContext.ContextData.GetValueOrDefault("rerollCardData", new List<CardData>())).Contains(CardData))
            {
                actions.Add(new PlayCardAction(this, CardData, PF.ActionType.Discard, powerIndex: DiscardIndex));
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
        if (!Game.CheckContext.ContextData.ContainsKey("rerollCardData"))
            Game.CheckContext.ContextData["rerollCardData"] = new List<CardData>();
        List<CardData> rerollSources = (List<CardData>)Game.CheckContext.ContextData["rerollCardData"];

        // May not play an Offhand boon this encounter.
        Game.EncounterContext.ProhibitedTraits.Add("Offhand");

        // Reveal to use Strength or Melee + 1d8.
        if (powerIndex == RevealIndex)
        {
            (int die, int bonus) meleeSkill = Game.TurnContext.CurrentPC.GetSkill(PF.Skill.Melee);
            (int die, int bonus) strSkill = Game.TurnContext.CurrentPC.GetAttr(PF.Skill.Strength);

            var (skill, die, bonus) = meleeSkill.die >= strSkill.die ? (PF.Skill.Melee, meleeSkill.die, meleeSkill.bonus) : (PF.Skill.Strength, strSkill.die, strSkill.bonus);
            Game.CheckContext.UsedSkill = skill;
            Game.CheckContext.DicePool.AddDice(1, die, bonus);
            Game.CheckContext.DicePool.AddDice(1, 8);

            rerollSources.Add(CardData);
        }

        // Discard to reroll.
        if (powerIndex == DiscardIndex)
        {
            rerollSources.Remove(CardData);
            Game.CheckContext.ContextData["doReroll"] = true;
        }
    }
}
