using PACG.SharedAPI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class PlayerCharacter
    {
        // --- Dependency Injections
        private readonly CardManager cardManager;

        public CharacterData characterData;

        private readonly Deck deck = new();

        public PlayerCharacter(CharacterData characterData, CardManager cardManager)
        {
            this.characterData = characterData;
            this.cardManager = cardManager;
        }

        // ==============================================================================
        // CONVENIENCE FUNCTIONS - PASS-THROUGHS TO CardManager
        // ==============================================================================
        public IReadOnlyList<CardInstance> Hand => cardManager.GetCardsInHand(this);
        public IReadOnlyList<CardInstance> Discards => cardManager.GetCardsOwnedBy(this, CardLocation.Discard);
        public IReadOnlyList<CardInstance> BuriedCards => cardManager.GetCardsOwnedBy(this, CardLocation.Buried);
        public IReadOnlyList<CardInstance> DisplayedCards => cardManager.GetCardsOwnedBy(this, CardLocation.Displayed);
        public IReadOnlyList<CardInstance> RecoveryCards => cardManager.GetCardsOwnedBy(this, CardLocation.Recovery);

        // ==============================================================================
        // SKILLS AND ATTRIBUTES
        // ==============================================================================
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

            foreach (var skill in skills)
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

        // ==============================================================================
        // CARD MOVEMENT INVOLVING THE PLAYER'S DECK
        // ==============================================================================
        public void Recharge(CardInstance card)
        {
            if (card == null || card.Owner != this) return;
            cardManager.MoveCard(card, CardLocation.Deck);
            deck.Recharge(card);
        }

        public void Reload(CardInstance card)
        {
            if (card == null || card.Owner != this) return;
            cardManager.MoveCard(card, CardLocation.Deck);
            deck.Reload(card);
        }

        public void DrawFromDeck()
        {
            if (deck.Count == 0)
            {
                // TODO: Handle character death.
                Debug.Log($"{characterData.characterName} must draw but has no more cards left. {characterData.characterName} dies!");
                return;
            }
            var card = deck.DrawCard();
            card.Owner = this;
            cardManager.MoveCard(card, CardLocation.Hand);
        }

        public void DrawToHandSize()
        {
            int cardsToDraw = characterData.handSize - Hand.Count;
            Debug.Log($"{characterData.characterName} drawing {cardsToDraw} up to {characterData.handSize}");
            if (cardsToDraw < 0) return;

            if (cardsToDraw > deck.Count)
            {
                // TODO: Handle character death.
                Debug.Log($"{characterData.characterName} must draw {cardsToDraw} but only has {deck.Count} left. {characterData.characterName} dies!");
                return;
            }

            for (int i = 0; i < cardsToDraw; i++) DrawFromDeck();
        }

        public void ShuffleIntoDeck(CardInstance card)
        {
            if (card == null) return;
            cardManager.MoveCard(card, CardLocation.Deck);
            deck.ShuffleIn(card);
        }
    }
}
