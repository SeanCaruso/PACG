using PACG.Data;
using PACG.SharedAPI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PACG.Gameplay
{
    public class PlayerCharacter : ICard, IExaminable
    {
        // ICard properties
        public string Name => CharacterData.CharacterName;
        public List<string> Traits => CharacterData.Traits;
        
        public CharacterData CharacterData { get; }
        private CharacterLogicBase Logic { get; }
        public Deck Deck { get; }

        public Dictionary<PF.Skill, (int die, int bonus)> Skills { get; } = new();
        public (int die, int bonus) GetSkill(PF.Skill skill) => Skills.GetValueOrDefault(skill, (4, 0));

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

            foreach (var attr in CharacterData.Attributes)
            {
                Skills[attr.Attribute] = (attr.Die, attr.Bonus);
            }

            foreach (var skill in CharacterData.Skills)
            {
                Skills[skill.Skill] = (Skills[skill.Attribute].die, skill.Bonus);
            }
        }

        // ==============================================================================
        // SKILLS AND ATTRIBUTES
        // ==============================================================================
        public bool IsProficient(CardData card)
        {
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var proficiency in CharacterData.Proficiencies)
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

        public (PF.Skill skill, int die, int bonus) GetBestSkill(params PF.Skill[] skills)
        {
            if (skills.Length == 0) return (PF.Skill.Strength, 4, 0);
            
            var bestSkill = skills[0];
            int bestDie = 4, bestBonus = 0;
            var bestAvg = 2.5;

            foreach (var skill in skills)
            {
                if (!Skills.TryGetValue(skill, out var value))
                    value = (4, 0);

                var avg = (value.die / 2.0) + 0.5 + value.bonus;
                if (!(avg > bestAvg)) continue;
                bestSkill = skill;
                bestDie = value.die;
                bestBonus = value.bonus;
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
                    $"{CharacterData.CharacterName} must draw but has no more cards left. {CharacterData.CharacterName} dies!");
                return;
            }

            var card = Deck.DrawCard();
            AddToHand(card);

            GameEvents.RaisePlayerDeckCountChanged(Deck.Count);
        }

        public void DrawInitialHand()
        {
            // TODO: Handle multiple favored card types.
            var fav = CharacterData.FavoredCards[0];

            var card = Deck.DrawFirstCardWith(fav.CardType, fav.Trait);
            if (card != null)
                AddToHand(card);

            DrawToHandSize();
        }

        public void DrawToHandSize()
        {
            var cardsToDraw = CharacterData.HandSize - Hand.Count;
            Debug.Log($"{CharacterData.CharacterName} drawing {cardsToDraw} up to {CharacterData.HandSize}");
            if (cardsToDraw < 0) return;

            if (cardsToDraw > Deck.Count)
            {
                // TODO: Handle character death.
                Debug.Log(
                    $"{CharacterData.CharacterName} must draw {cardsToDraw} but only has {Deck.Count} left. {CharacterData.CharacterName} dies!");
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
        public IReadOnlyList<CardInstance> AllCards => _cardManager.GetCardsOwnedBy(this);
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
