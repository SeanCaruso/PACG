using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PACG.Gameplay
{
    public class CheckProcessor : IProcessor
    {
        private readonly ContextManager _contexts;
        private readonly LogicRegistry _logic;
        private readonly GameFlowManager _gameFlowManager;
        private IResolvable Resolvable { get; }

        public CheckProcessor(IResolvable resolvable, ContextManager contextManager, LogicRegistry logicRegistry, GameFlowManager gameFlowManager)
        {
            Resolvable = resolvable;
            _contexts = contextManager;
            _logic = logicRegistry;
            _gameFlowManager = gameFlowManager;
        }

        public void Execute()
        {
            if (Resolvable is CombatResolvable combat)
                ResolveCombatCheck(combat.Character, combat.Difficulty);
        }

        private void ResolveCombatCheck(PlayerCharacter pc, int dc)
        {
            CheckContext check = _contexts.CheckContext;

            // Add blessing dice.
            check.DicePool.AddDice(check.BlessingCount, pc.GetSkill(check.UsedSkill).die);
            check.CheckPhase = CheckPhase.RollDice;
            int rollTotal = check.DicePool.Roll();
            check.CheckResult = new(rollTotal, dc, pc, check.UsedSkill, check.Traits);

            bool needsReroll = check.CheckResult.MarginOfSuccess < _contexts.EncounterContext.EncounteredCard.Data.rerollThreshold;
            bool hasRerollOptions = false;
            var cardsToCheck = pc.Hand.Union(pc.DisplayedCards);
            foreach (var card in cardsToCheck)
            {
                if (_logic.GetPlayableLogic(card).GetAvailableActions().Count > 0)
                {
                    hasRerollOptions = true;
                    break;
                }
            }

            // No playable cards allow rerolls... check if a played card set the context.
            hasRerollOptions |= ((List<CardInstance>)_contexts.CheckContext.ContextData.GetValueOrDefault("rerollCards", new List<CardInstance>())).Count > 0;

            if (needsReroll && hasRerollOptions)
            {
                RerollResolvable rerollResolvable = new(_logic, pc, check);
                _contexts.NewResolution(new(rerollResolvable));
                return; // We're done - GameFlowManager takes over.
            }

            ApplyCheckResult(check, dc);
        }

        private void ApplyCheckResult(CheckContext check, int dc)
        {
            if (check.CheckResult.WasSuccess)
            {
                Debug.Log($"Rolled {check.CheckResult.FinalRollTotal} vs. {dc} - Success!");
            }
            else if (false /* avenge? */)
            { }
            else
            {
                check.CheckPhase = CheckPhase.SufferDamage;

                DamageResolvable damageResolvable = new(_logic, check.Resolvable.Character, -check.CheckResult.MarginOfSuccess);
                _contexts.NewResolution(new(damageResolvable));
                Debug.Log($"Rolled {check.CheckResult.FinalRollTotal} vs. {dc} - Take {damageResolvable.Amount} damage!");
                return; // We're done - GameFlowManager takes over.
            }
        }
    }
}
