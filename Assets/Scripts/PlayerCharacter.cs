using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter
{
    public CharacterData characterData;

    public Deck deck;
    public List<CardData> hand = new();
    public List<CardData> discards = new();
    public List<CardData> buriedCards = new();
    public List<CardData> displayedCards = new();

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

    public void MoveCard(CardData card, PF.ActionType action)
    {
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

    private void RemoveCardFromAllZones(CardData card)
    {
        hand.Remove(card);
        discards.Remove(card);
        buriedCards.Remove(card);
        displayedCards.Remove(card);
    }
}
