using PACG.Data;
using PACG.SharedAPI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PACG.Gameplay
{
    public class PlayerCharacter : IExaminable
    {
        public CharacterData CharacterData { get; }
        private CharacterLogicBase Logic { get; }
        public Deck Deck { get; }

        public string DisplayName => CharacterData.characterName;

        // --- Dependency Injections
        private readonly CardManager _cardManager;
        private readonly ContextManager _contexts;

        public PlayerCharacter(CharacterData characterData, CharacterLogicBase logic, GameServices gameServices)
        {
            CharacterData = characterData;
            Logic = logic;
            Deck = new Deck(gameServices.Cards);

            _cardManager = gameServices.Cards;
            _contexts = gameServices.Contexts;
        }

        // ==============================================================================
        // SKILLS AND ATTRIBUTES
        // ==============================================================================
        public bool IsProficient(CardData card)
        {
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var proficiency in CharacterData.proficiencies)
            {
                // Check if card type matches (or doesn't matter)
                var typeMatches = proficiency.CardType == card.cardType ||
                                  proficiency.CardType == PF.CardType.None;
                
                // Check if trait matches (or doesn't matter)
                var traitMatches = card.traits.Contains(proficiency.Trait) ||
                                   string.IsNullOrEmpty(proficiency.Trait);
                
                if (typeMatches && traitMatches)
                    return true;
            }

            return false;
        }

        private (int die, int bonus) GetAttr(PF.Skill attr)
        {
            foreach (var charAttr in CharacterData.attributes.Where(charAttr => charAttr.attribute == attr))
            {
                return (charAttr.die, charAttr.bonus);
            }

            return (4, 0); // Default of 1d4.
        }

        public (int die, int bonus) GetSkill(PF.Skill skill)
        {
            foreach (var charSkill in CharacterData.skills.Where(charSkill => charSkill.skill == skill))
            {
                var (attrDie, attrBonus) = GetAttr(charSkill.attribute);
                return (attrDie, attrBonus + charSkill.bonus);
            }

            return (4, 0); // Default of 1d4.
        }

        public (PF.Skill, int die, int bonus) GetBestSkill(params PF.Skill[] skills)
        {
            var bestSkill = skills[0];
            int bestDie = 4, bestBonus = 0;
            var bestAvg = 2.5;

            foreach (var skill in skills)
            {
                var (die, bonus) = GetSkill(skill);
                if (die == 4 && bonus == 0)
                {
                    (die, bonus) = GetAttr(skill);
                }

                var avg = (die / 2.0) + 0.5 + bonus;
                if (!(avg > bestAvg)) continue;
                bestSkill = skill;
                bestDie = die;
                bestBonus = bonus;
                bestAvg = avg;
            }

            return (bestSkill, bestDie, bestBonus);
        }

        // ==============================================================================
        // CARD MOVEMENT INVOLVING THE PLAYER'S DECK
        // ==============================================================================
        public void Recharge(CardInstance card)
        {
            if (card == null || card.Owner != this) return;
            _cardManager.MoveCard(card, CardLocation.Deck);
            Deck.Recharge(card);
        }

        public void Reload(CardInstance card)
        {
            if (card == null || card.Owner != this) return;
            _cardManager.MoveCard(card, CardLocation.Deck);
            Deck.Reload(card);
        }

        public void AddToHand(CardInstance card)
        {
            card.Owner = this;
            _cardManager.MoveCard(card, CardLocation.Hand);
        }

        public void DrawFromDeck()
        {
            if (Deck.Count == 0)
            {
                // TODO: Handle character death.
                Debug.Log(
                    $"{CharacterData.characterName} must draw but has no more cards left. {CharacterData.characterName} dies!");
                return;
            }

            var card = Deck.DrawCard();
            AddToHand(card);

            GameEvents.RaisePlayerDeckCountChanged(Deck.Count);
        }

        public void DrawInitialHand()
        {
            // TODO: Handle multiple favored card types.
            var fav = CharacterData.favoredCards[0];

            var card = Deck.DrawFirstCardWith(fav.cardType, fav.trait);
            if (card != null)
                AddToHand(card);

            DrawToHandSize();
        }

        public void DrawToHandSize()
        {
            var cardsToDraw = CharacterData.handSize - Hand.Count;
            Debug.Log($"{CharacterData.characterName} drawing {cardsToDraw} up to {CharacterData.handSize}");
            if (cardsToDraw < 0) return;

            if (cardsToDraw > Deck.Count)
            {
                // TODO: Handle character death.
                Debug.Log(
                    $"{CharacterData.characterName} must draw {cardsToDraw} but only has {Deck.Count} left. {CharacterData.characterName} dies!");
                return;
            }

            for (var i = 0; i < cardsToDraw; i++)
                DrawFromDeck();
        }

        public void ShuffleIntoDeck(CardInstance card)
        {
            if (card == null) return;
            _cardManager.MoveCard(card, CardLocation.Deck);
            Deck.ShuffleIn(card);
        }

        // ==============================================================================
        // CONVENIENCE FUNCTIONS
        // ==============================================================================

        // Pass-throughs to CardManager
        public IReadOnlyList<CardInstance> Hand => _cardManager.GetCardsInHand(this);
        public IReadOnlyList<CardInstance> Discards => _cardManager.GetCardsOwnedBy(this, CardLocation.Discard);
        public IReadOnlyList<CardInstance> BuriedCards => _cardManager.GetCardsOwnedBy(this, CardLocation.Buried);
        public IReadOnlyList<CardInstance> DisplayedCards => _cardManager.GetCardsOwnedBy(this, CardLocation.Displayed);
        public IReadOnlyList<CardInstance> RecoveryCards => _cardManager.GetCardsOwnedBy(this, CardLocation.Recovery);
        public IReadOnlyList<CardInstance> DeckCards => _cardManager.GetCardsOwnedBy(this, CardLocation.Deck);

        // Pass-throughs to ContextManager
        public IReadOnlyList<PlayerCharacter> LocalCharacters =>
            _contexts.GameContext.GetCharactersAt(Location).Except(new[] { this }).ToList();
        public Location Location => _contexts.GameContext.GetPcLocation(this);
        public void Move(Location newLoc) => _contexts.GameContext.MoveCharacter(this, newLoc);

        // Facade pattern for CharacterLogic
        public List<IResolvable> GetStartOfTurnResolvables() => Logic.GetStartOfTurnResolvables(this);
        public List<IResolvable> GetEndOfTurnResolvables() => Logic.GetEndOfTurnResolvables(this);
    }
}
