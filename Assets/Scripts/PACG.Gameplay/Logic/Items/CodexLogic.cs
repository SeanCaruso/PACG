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

        public override CheckModifier GetCheckModifier(IStagedAction action)
        {
            switch (action.ActionType)
            {
                case ActionType.Reveal:
                    return new CheckModifier(action.Card)
                    {
                        AddedBonus = 1
                    };
                case ActionType.Discard:
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

            // Reveal on your check to acquire.
            if (_contexts.CurrentResolvable is CheckResolvable
                {
                    Card: CardInstance { IsBoon: true }
                } acquireResolvable
                && !acquireResolvable.IsCardTypeStaged(card.CardType)
                && acquireResolvable.Character == card.Owner)
            {
                actions.Add(new PlayCardAction(card, ActionType.Reveal));
            }

            // Discard on a local check to acquire
            if (_contexts.CurrentResolvable is CheckResolvable { Card: CardInstance { IsBoon: true } } localResolvable
                && !localResolvable.IsCardTypeStaged(card.CardType)
                && localResolvable.Character.LocalCharacters.Contains(card.Owner))
            {
                actions.Add(new PlayCardAction(card, ActionType.Discard));
            }

            return actions;
        }
    }
}
