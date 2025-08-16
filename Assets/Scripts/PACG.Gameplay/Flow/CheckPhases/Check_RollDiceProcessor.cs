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
        private readonly ContextManager _contexts;

        public Check_RollDiceProcessor(GameServices gameServices) : base(gameServices)
        {
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

            foreach (var effect in check.ExploreEffects)
            {
                effect.ApplyToCheck(check);
            }

            // Add blessing dice.
            check.DicePool.AddDice(check.BlessingCount, pc.GetSkill(check.UsedSkill).die);
            var rollTotal = check.DicePool.Roll();
            check.CheckResult = new CheckResult(rollTotal, dc, pc, check.UsedSkill, check.Traits);

            var needsReroll = check.CheckResult.MarginOfSuccess < _contexts.EncounterContext.CardData.rerollThreshold;
            var cardsToCheck = pc.Hand.Union(pc.DisplayedCards);
            var hasRerollOptions = cardsToCheck.Any(card => card.GetAvailableActions().Count > 0);

            // No playable cards allow rerolls... check if a played card set the context.
            hasRerollOptions |= ((List<CardLogicBase>)_contexts.CheckContext.ContextData.GetValueOrDefault("rerollCards", new List<CardLogicBase>())).Count > 0;
            
            // No reroll options. We're done!
            if (!needsReroll || !hasRerollOptions)
            {
                GameEvents.SetStatusText($"Rolled {check.DicePool}: {rollTotal}");
                return;
            }
            
            GameEvents.SetStatusText($"Rolled {check.DicePool}: {rollTotal}... Reroll?");
            // ... otherwise, create a reroll resolvable.
            RerollResolvable rerollResolvable = new(pc, check);
            _contexts.NewResolvable(rerollResolvable);
        }
    }
}
