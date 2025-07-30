using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter
{
    public CharacterData characterData;

    public Deck deck;
    public readonly List<CardInstance> hand = new();
    public readonly List<CardInstance> discards = new();
    public readonly List<CardInstance> buriedCards = new();
    public readonly List<CardInstance> displayedCards = new();

    public bool IsProficient(PF.CardType cardType) => characterData.proficiencies.Contains(cardType);

    public (int die, int bonus) GetAttr(PF.Skill attr)
    {
        foreach (var charAttr in characterData.attributes)
        {
            if (charAttr.attribute == attr)
            {
                return (charAttr.die, charAttr.bonus);
            }
        }
        return (4, 0); // Default of 1d4.
    }

    public (int die, int bonus) GetSkill(PF.Skill skill)
    {
        foreach (var charSkill in characterData.skills)
        {
            if (charSkill.skill == skill)
            {
                (int attrDie, int attrBonus) = GetAttr(charSkill.attribute);
                return (attrDie, attrBonus + charSkill.bonus);
            }
        }
        return (4, 0); // Default of 1d4.
    }

    public (PF.Skill, int die, int bonus) GetBestSkill(params PF.Skill[] skills)
    {
        PF.Skill bestSkill = skills[0];
        int bestDie = 4, bestBonus = 0;
        double bestAvg = 2.5;

        foreach (var skill in  skills)
        {
            var (die, bonus) = GetSkill(skill);
            if (die == 4 && bonus == 0)
            {
                (die, bonus) = GetAttr(skill);
            }

            double avg = (die / 2.0) + 0.5 + bonus;
            if (avg > bestAvg)
            {
                bestSkill = skill;
                bestDie = die;
                bestBonus = bonus;
                bestAvg = avg;
            }
        }

        return (bestSkill, bestDie, bestBonus);
    }

    public void MoveCard(CardInstance card, PF.ActionType action)
    {
        Debug.Log($"Moving {card} via {action}");
        if (action == PF.ActionType.Reveal)
            return;

        RemoveCardFromAllZones(card);

        switch (action)
        {
            case PF.ActionType.Banish:
                break;
            case PF.ActionType.Bury:
                buriedCards.Add(card); break;
            case PF.ActionType.Discard:
                discards.Add(card); break;
            case PF.ActionType.Display:
                displayedCards.Add(card); break;
            case PF.ActionType.Draw:
                hand.Add(card); break;
            case PF.ActionType.Recharge:
                deck.Recharge(card); break;
            case PF.ActionType.Reload:
                deck.Reload(card); break;
            default:
                Debug.LogError($"MoveCard --- Unknown action: {action}"); break;
        }
    }

    public void MoveCard(CardInstance card, List<CardInstance> location)
    {
        Debug.Log($"Moving {card} to {location}");
        RemoveCardFromAllZones(card);
        location.Add(card);
    }

    public List<CardInstance> FindCard(CardInstance card)
    {
        if (hand.Contains(card)) return hand;
        if (discards.Contains(card)) return discards;
        if (displayedCards.Contains(card)) return displayedCards;
        if (buriedCards.Contains(card)) return buriedCards;

        return null;
    }

    private void RemoveCardFromAllZones(CardInstance card)
    {
        hand.Remove(card);
        discards.Remove(card);
        buriedCards.Remove(card);
        displayedCards.Remove(card);
    }
}
