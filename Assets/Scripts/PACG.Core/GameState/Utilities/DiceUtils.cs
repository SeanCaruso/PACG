using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PACG.Core
{
    public static class DiceUtils
    {
        public static int Roll(int sides) => Random.Range(1, sides + 1);
        public static int Roll(int count, int sides)
        {
            var total = 0;
            for (var i = 0; i < count; i++)
            {
                total += Random.Range(1, sides + 1);
            }
            return total;
        }
    }

    public class DicePool
    {
        private readonly Dictionary<int, int> _dice = new(); // Key: Sides; Value: count
        private int _bonus;

        public void AddDice(int count, int sides, int bonus = 0)
        {
            _dice.TryAdd(sides, 0);

            _dice[sides] += count;
            _bonus += bonus;
        }
    
        public void AddBonus(int bonus) => _bonus += bonus;

        public int NumDice(params int[] sides)
        {
            return sides.Sum(side => _dice.GetValueOrDefault(side, 0));
        }

        public int Roll()
        {
            var rollResult = _bonus;
            foreach(var (sides, count) in _dice)
            {
                rollResult += DiceUtils.Roll(count, sides);
            }
            Debug.Log($"Rolling {ToString()} ==> {rollResult}");
            return rollResult;
        }

        public override string ToString()
        {
            var retval = "";
            foreach (var sides in _dice.Keys.OrderByDescending(d => d))
            {
                retval += retval != "" ? " + " : "";
                retval += $"{_dice[sides]}d{sides}";
            }
            retval += _bonus == 0 ? "" : $" + {_bonus}";
            return retval;
        }
    }
}
