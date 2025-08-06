using PACG.SharedAPI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class PlayerCharacter : IDisposable
    {
        // --- Events
        public event Action HandChanged;

        // --- Dependency Injections
        private readonly CardManager cardManager;

        public CharacterData characterData;

        private readonly Deck deck = new();

        public List<CardInstance> Hand { get; } = new();
        public List<CardInstance> Discards { get; } = new();
        public List<CardInstance> BuriedCards { get; } = new();
        public List<CardInstance> DisplayedCards { get; } = new();
        public List<CardInstance> RecoveryCards { get; } = new();

        public bool IsProficient(PF.CardType cardType) => characterData.proficiencies.Contains(cardType);

        public PlayerCharacter(CharacterData characterData, CardManager cardManager)
        {
            this.characterData = characterData;
            this.cardManager = cardManager;
            GameEvents.CardLocationChanged += UpdateCardLists;
        }

        public void Dispose()
        {
            GameEvents.CardLocationChanged -= UpdateCardLists;
        }

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

        // --- Card Power Action Types --------------------------------------------------
        public void Banish(CardInstance card)
        {
            if (card == null || card.Owner != this) return;
            cardManager.MoveCard(card, CardLocation.Vault);
        }

        public void Bury(CardInstance card)
        {
            if (card == null || card.Owner != this) return;
            cardManager.MoveCard(card, CardLocation.Buried);
        }

        public void Discard(CardInstance card)
        {
            if (card == null || card.Owner != this) return;
            cardManager.MoveCard(card, CardLocation.Discard);
        }

        public void Display(CardInstance card)
        {
            if (card == null || card.Owner != this) return;
            cardManager.MoveCard(card, CardLocation.Displayed);
        }

        public void Draw(CardInstance card)
        {
            // This is specific to the Draw card power, so enforce card ownership.
            if (card == null || card.Owner != this) return;

            card.Owner = this;
            cardManager.MoveCard(card, CardLocation.Hand);
        }

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
        // ---------------------------------------------------------------------------------

        // --- Convenience Functions for Card Movement -------------------------------------
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
            cardManager.MoveCard(card, CardLocation.Hand, true);
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
            cardManager.MoveCard(card, CardLocation.Deck, true);
            deck.ShuffleIn(card);
        }
        // ----------------------------------------------------------------------------------

        public void MoveCard(CardInstance card, PF.ActionType action)
        {
            if (card == null || card.Owner != this)
            {
                Debug.LogWarning($"Attempted to {action} a card not owned by {characterData.characterName}.");
            }

            switch (action)
            {
                case PF.ActionType.Banish:
                    Banish(card);
                    break;
                case PF.ActionType.Bury:
                    Bury(card);
                    break;
                case PF.ActionType.Discard:
                    Discard(card);
                    break;
                case PF.ActionType.Display:
                    Display(card);
                    break;
                case PF.ActionType.Draw:
                    Draw(card);
                    break;
                case PF.ActionType.Recharge:
                    Recharge(card);
                    break;
                case PF.ActionType.Reload:
                    Reload(card);
                    break;
                case PF.ActionType.Reveal:
                    // No action necessary.
                    break;
                default:
                    Debug.LogError($"Unsupported action: {action}!");
                    break;
            }
        }

        private void UpdateCardLists(CardInstance card)
        {
            // We only care about our own cards.
            if (card.Owner != this) return;

            // Remove from all locations.
            bool wasInHand = Hand.Remove(card);
            Discards.Remove(card);
            BuriedCards.Remove(card);
            DisplayedCards.Remove(card);

            bool isNowInHand = false;
            switch (card.CurrentLocation)
            {
                case CardLocation.Buried:
                    BuriedCards.Add(card);
                    break;
                case CardLocation.Deck:
                    // The Deck class handles its own logic.
                    break;
                case CardLocation.Discard:
                    Discards.Add(card);
                    break;
                case CardLocation.Displayed:
                    DisplayedCards.Add(card);
                    break;
                case CardLocation.Hand:
                    isNowInHand = true;
                    Hand.Add(card);
                    break;
            }

            if (wasInHand != isNowInHand) HandChanged?.Invoke();
        }
    }
}
