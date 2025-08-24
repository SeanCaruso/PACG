using System.Collections.Generic;
using System.Linq;
using PACG.Core;

namespace PACG.Gameplay
{
    public static class DicePoolBuilder
    {
        public static DicePool Build(CheckContext checkContext, IReadOnlyList<IStagedAction> actions)
        {
            var modifiers = new List<CheckModifier>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var action in actions)
            {
                var modifier = action.Card.Logic?.GetCheckModifier(action);
                if (modifier != null)
                    modifiers.Add(modifier);
            }

            var dicePool = new DicePool();

            // 1. Apply any persistent explore effects.
            foreach (var effect in checkContext.ExploreEffects)
            {
                effect.ApplyTo(checkContext, dicePool);
            }

            // 2. Find and apply the used skill die and blessings.
            // Look for a die override.
            var dieOverride = modifiers.FirstOrDefault(m => m.DieOverride != null)?.DieOverride;
            // Use it if we found one, otherwise get the skill die for the used skill.
            var skillDie = dieOverride ??
                           checkContext.Character.GetSkill(checkContext.UsedSkill).die;
            var skillBonus = checkContext.Character.GetSkill(checkContext.UsedSkill).bonus;

            var totalSkillDice = 1 + modifiers.Sum(modifier => modifier.SkillDiceToAdd);
            dicePool.AddDice(totalSkillDice, skillDie, skillBonus);
            
            // 3. Apply any other dice modifiers.
            foreach (var modifier in modifiers)
            {
                foreach (var die in modifier.AddedDice)
                {
                    dicePool.AddDice(1, die);
                }
                dicePool.AddBonus(modifier.AddedBonus);
            }

            return dicePool;
        }
    }
}
