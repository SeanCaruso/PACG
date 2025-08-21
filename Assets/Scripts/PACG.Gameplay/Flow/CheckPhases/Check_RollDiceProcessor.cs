using System.Collections.Generic;
using System.Linq;
using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    /// <summary>
    /// Check sub-processor to handle rolling dice (including after rerolls).
    /// </summary>
    public class Check_RollDiceProcessor : BaseProcessor
    {
        private readonly ActionStagingManager _asm;
        private readonly ContextManager _contexts;

        public Check_RollDiceProcessor(GameServices gameServices) : base(gameServices)
        {
            _asm = gameServices.ASM;
            _contexts = gameServices.Contexts;
        }

        protected override void OnExecute()
        {
            var resolvable = _contexts.CheckContext.Resolvable;
            if (resolvable == null)
            {
                Debug.LogError($"[{GetType().Name}] Resolvable is null!");
                return;
            }

            var pc = resolvable.Character;
            var dc = resolvable.Difficulty;
            var check = _contexts.CheckContext;

            // foreach (var effect in check.ExploreEffects)
            // {
            //     effect.ApplyTo(check);
            // }
            //
            // // Add the skill set by the skill selection dialog.
            // var usedSkill = pc.GetSkill(check.UsedSkill);
            //
            // if (check.DieOverride != null)
            //     usedSkill.die = check.DieOverride.Value;
            //
            // check.DicePool.AddDice(1, usedSkill.die, usedSkill.bonus);
            //
            // // Add blessing dice.
            // check.DicePool.AddDice(check.BlessingCount, usedSkill.die);

            var dicePool = _asm.GetStagedDicePool();

            var rollTotal = dicePool.Roll();
            check.CheckResult = new CheckResult(rollTotal, dc, pc, check.UsedSkill, check.Traits);

            var needsReroll = check.CheckResult.MarginOfSuccess < _contexts.EncounterContext.CardData.rerollThreshold;
            var cardsToCheck = pc.Hand.Union(pc.DisplayedCards);
            var cardInstances = cardsToCheck.ToList();
            var hasRerollOptions = cardInstances.Any(card => card.GetAvailableActions().Count > 0);

            foreach (var card in cardInstances.Where(card => card.GetAvailableActions().Count > 0))
            {
                Debug.Log($"{card} has a reroll action.");
            }

            // No playable cards allow rerolls... check if a played card set the context.
            hasRerollOptions |=
                ((List<CardLogicBase>)_contexts.CheckContext.ContextData.GetValueOrDefault("rerollCards",
                    new List<CardLogicBase>())).Count > 0;

            // No reroll options. We're done!
            if (!needsReroll || !hasRerollOptions)
            {
                GameEvents.SetStatusText($"Rolled {dicePool}: {rollTotal}");
                return;
            }

            GameEvents.SetStatusText($"Rolled {dicePool}: {rollTotal}... Reroll?");
            // ... otherwise, create a reroll resolvable.
            RerollResolvable rerollResolvable = new(pc, dicePool, check);
            _contexts.NewResolvable(rerollResolvable);
        }
    }
}
