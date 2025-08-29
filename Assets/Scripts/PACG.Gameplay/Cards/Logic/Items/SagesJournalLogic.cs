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

        public override CheckModifier GetCheckModifier(IStagedAction action)
        {
            switch (action.ActionType)
            {
                case ActionType.Reveal:
                    return new CheckModifier(action.Card)
                    {
                        AddedDice = new List<int> { 4 }
                    };
                case ActionType.Bury:
                    var (die, bonus) = action.Card.Owner.GetSkill(Skill.Knowledge);
                    return new CheckModifier(action.Card)
                    {
                        AddedDice = new List<int> { die },
                        AddedBonus = bonus
                    };
                default:
                    return null;
            }
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();

            // Reveal on your check against a story bane.
            if (_contexts.CurrentResolvable is CheckResolvable
                {
                    Card: CardInstance { IsStoryBane: true }
                } storyBaneResolvable
                && !storyBaneResolvable.IsCardTypeStaged(card.CardType)
                && storyBaneResolvable.Character == card.Owner)
            {
                actions.Add(new PlayCardAction(card, ActionType.Reveal));
            }

            // Bury on a local check against a bane.
            if (_contexts.CurrentResolvable is CheckResolvable { Card: CardInstance { IsBane: true } } baneResolvable
                && !baneResolvable.IsCardTypeStaged(card.CardType)
                && baneResolvable.Character.LocalCharacters.Contains(card.Owner))
            {
                actions.Add(new PlayCardAction(card, ActionType.Bury));
            }

            return actions;
        }
    }
}
