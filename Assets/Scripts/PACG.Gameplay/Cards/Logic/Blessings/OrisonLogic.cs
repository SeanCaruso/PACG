using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public class OrisonLogic : CardLogicBase
    {
        private readonly ContextManager _contexts;

        public OrisonLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        public override void Execute(CardInstance card, IStagedAction action, DicePool dicePool)
        {
            if (action is not ExploreAction)
                _contexts.CheckContext.BlessingCount++;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();
            if (CanBless(card))
            {
                actions.Add(new PlayCardAction(card, PF.ActionType.Discard));

                if (_contexts.TurnContext.HourCard.Data.cardLevel == 0)
                    actions.Add(new PlayCardAction(card, PF.ActionType.Recharge));
            }
            else if (_contexts.IsExplorePossible && _contexts.TurnContext.Character == card.Owner)
            {
                actions.Add(new ExploreAction(card, PF.ActionType.Discard));
            }

            return actions;
        }

        // We can bless on a local check.
        private bool CanBless(CardInstance card) => (
            _contexts.CheckContext != null &&
            _contexts.CurrentResolvable is CheckResolvable &&
            !_contexts.CheckContext.StagedCardTypes.Contains(PF.CardType.Blessing) &&
            _contexts.CheckContext.Character.Location.Characters.Contains(card.Owner)
        );
    }
}
