using System.Collections.Generic;
using System.Linq;
using PACG.Core;

namespace PACG.Gameplay
{
    public class SagesJournalLogic : CardLogicBase
    {
        // Dependency injection
        private readonly ContextManager _contexts;

        public SagesJournalLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();

            // Reveal for +1d4 on your check against a story bane.
            if (_contexts.CurrentResolvable is CheckResolvable
                {
                    Card: CardInstance { IsStoryBane: true }
                } storyBaneResolvable
                && !storyBaneResolvable.IsCardTypeStaged(card.CardType)
                && storyBaneResolvable.Character == card.Owner)
            {
                var modifier = new CheckModifier(card)
                {
                    AddedDice = new List<int> { 4 }
                };
                actions.Add(new PlayCardAction(card, ActionType.Reveal, modifier));
            }

            // Bury for +Knowledge on a local check against a bane.
            if (_contexts.CurrentResolvable is CheckResolvable { Card: CardInstance { IsBane: true } } baneResolvable
                && !baneResolvable.IsCardTypeStaged(card.CardType)
                && baneResolvable.Character.LocalCharacters.Contains(card.Owner))
            {
                var (die, bonus) = card.Owner.GetSkill(Skill.Knowledge);
                var modifier = new CheckModifier(card)
                {
                    AddedDice = new List<int> { die },
                    AddedBonus = bonus
                };
                actions.Add(new PlayCardAction(card, ActionType.Bury, modifier));
            }

            return actions;
        }
    }
}
