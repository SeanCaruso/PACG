using System.Collections.Generic;
using System.Linq;
using PACG.Core;

namespace PACG.Gameplay
{
    public class CodexLogic : CardLogicBase
    {
        // Dependency injection
        private readonly ContextManager _contexts;

        public CodexLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();

            // Reveal for +1 on your check to acquire.
            if (_contexts.CurrentResolvable is CheckResolvable
                {
                    Card: CardInstance { IsBoon: true }
                } acquireResolvable
                && !acquireResolvable.IsCardTypeStaged(card.CardType)
                && acquireResolvable.Character == card.Owner)
            {
                var modifier = new CheckModifier(card) { AddedBonus = 1 };
                actions.Add(new PlayCardAction(card, ActionType.Reveal, modifier));
            }

            // Discard for +Knowledge on a local check to acquire
            // ReSharper disable once InvertIf
            if (_contexts.CurrentResolvable is CheckResolvable { Card: CardInstance { IsBoon: true } } localResolvable
                && !localResolvable.IsCardTypeStaged(card.CardType)
                && localResolvable.Character.LocalCharacters.Contains(card.Owner))
            {
                var (die, bonus) = card.Owner.GetSkill(Skill.Knowledge);
                var modifier = new CheckModifier(card) { AddedDice = new List<int> { die }, AddedBonus = bonus };
                actions.Add(new PlayCardAction(card, ActionType.Discard, modifier));
            }

            return actions;
        }
    }
}
