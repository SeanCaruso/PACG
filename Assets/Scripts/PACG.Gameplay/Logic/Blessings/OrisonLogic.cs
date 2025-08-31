using System.Collections.Generic;
using System.Linq;
using PACG.Core;
using PACG.Data;

namespace PACG.Gameplay
{
    public class OrisonLogic : CardLogicBase
    {
        private readonly ContextManager _contexts;

        public OrisonLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();
            if (CanBless(card))
            {
                var modifier = new CheckModifier(card) { SkillDiceToAdd = 1 };
                actions.Add(new PlayCardAction(card, ActionType.Discard, modifier));

                if (_contexts.TurnContext.HourCard.Data.cardLevel == 0)
                    actions.Add(new PlayCardAction(card, ActionType.Recharge, modifier));
            }
            else if (_contexts.IsExplorePossible && _contexts.TurnContext.Character == card.Owner)
            {
                actions.Add(new ExploreAction(card, ActionType.Discard));
            }

            return actions;
        }

        // We can bless on a local check.
        private bool CanBless(CardInstance card) =>
            _contexts.CheckContext != null
            && _contexts.CurrentResolvable is CheckResolvable resolvable
            && !resolvable.IsCardTypeStaged(card.CardType)
            && _contexts.CheckContext.Character.Location.Characters.Contains(card.Owner);
    }
}
