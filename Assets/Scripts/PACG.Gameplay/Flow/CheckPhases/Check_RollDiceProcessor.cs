using System.Collections.Generic;
using System.Linq;
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

            PlayerCharacter pc = resolvable.Character;
            int dc = resolvable.Difficulty;
            CheckContext check = _contexts.CheckContext;

            // Add blessing dice.
            check.DicePool.AddDice(check.BlessingCount, pc.GetSkill(check.UsedSkill).die);
            check.CheckPhase = CheckPhase.RollDice;
            int rollTotal = 8;// check.DicePool.Roll();
            check.CheckResult = new(rollTotal, dc, pc, check.UsedSkill, check.Traits);

            bool needsReroll = check.CheckResult.MarginOfSuccess < _contexts.EncounterContext.CardData.rerollThreshold;
            bool hasRerollOptions = false;
            var cardsToCheck = pc.Hand.Union(pc.DisplayedCards);
            foreach (var card in cardsToCheck)
            {
                if (card.GetAvailableActions().Count > 0)
                {
                    hasRerollOptions = true;
                    break;
                }
            }

            // No playable cards allow rerolls... check if a played card set the context.
            hasRerollOptions |= ((List<CardInstance>)_contexts.CheckContext.ContextData.GetValueOrDefault("rerollCards", new List<CardInstance>())).Count > 0;

            if (needsReroll && hasRerollOptions)
            {
                RerollResolvable rerollResolvable = new(pc, check);
                _contexts.NewResolvable(rerollResolvable);
                return; // We're done - GameFlowManager takes over.
            }
        }
    }
}
