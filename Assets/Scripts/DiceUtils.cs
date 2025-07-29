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
    private readonly Dictionary<int, int> dice = new(); // Key: Sides; Value: count
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

    public int NumDice(params int[] sides)
    {
        int total = 0;
        foreach (int side in sides)
        {
            total += dice.GetValueOrDefault(side, 0);
        }
        return total;
    }

    public int Roll()
    {
        int rollResult = bonus;
        foreach((int sides, int count) in dice)
        {
            rollResult += DiceUtils.Roll(count, sides);
        }
        Debug.Log($"Rolling {ToString()} ==> {rollResult}");
        return rollResult;
    }

    public override string ToString()
    {
        string retval = "";
        foreach ((int sides, int count) in dice)
        {
            retval += retval != "" ? " + " : "";
            retval += $"{count}d{sides}";
        }
        retval += bonus == 0 ? "" : $" + {bonus}";
        return retval;
    }
}