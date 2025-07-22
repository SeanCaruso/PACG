using System.Collections.Generic;
using UnityEngine;

public static class DiceUtils
{
    public static int Roll(int sides) => Random.Range(1, sides + 1);
    public static int Roll(int count, int sides)
    {
        int total = 0;
        for (int i = 0; i < count; i++)
        {
            total += Random.Range(1, sides + 1);
        }
        return total;
    }
}

public class DicePool
{
    private Dictionary<int, int> dice = new();
    private int bonus = 0;

    public void AddDice(int count, int sides, int bonus = 0)
    {
        if (!dice.ContainsKey(sides))
        {
            dice[sides] = 0;
        }

        dice[sides] += count;
        this.bonus += bonus;
    }

    public int Roll()
    {
        int rollResult = bonus;
        foreach((int sides, int count) in dice)
        {
            rollResult += DiceUtils.Roll(count, sides);
        }
        return rollResult;
    }
}