using System.Collections.Generic;
using System.Linq;
using PACG.Core;
using PACG.Data;

namespace PACG.Gameplay
{
    public class SleepLogic : CardLogicBase
    {
        // Dependency injection
        private readonly ContextManager _contexts;
        private readonly GameServices _gameServices;

        public SleepLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameServices = gameServices;
        }

        public override CheckModifier GetCheckModifier(IStagedAction action)
        {
            if (_contexts.CurrentResolvable is not CheckResolvable) return null;

            return new CheckModifier(action.Card)
            {
                AddedDice = new List<int> { 6 }
            };
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();

            if (CanBanishOnCheck(card) || CanBanishToEvade(card))
                actions.Add(new PlayCardAction(card, ActionType.Banish));

            return actions;
        }

        private bool CanBanishToEvade(CardInstance card) =>
            _contexts.EncounterContext is
            {
                CurrentPhase: EncounterPhase.Evasion,
                CardData: { cardType: CardType.Monster }
            }
            && card.Owner.LocalCharacters.Contains(_contexts.EncounterContext.Character);

        private bool CanBanishOnCheck(CardInstance card) =>
            _contexts.CurrentResolvable is CheckResolvable resolvable
            && resolvable.Card.CardType is CardType.Monster or CardType.Ally
            && resolvable.Character.LocalCharacters.Contains(card.Owner)
            && !resolvable.IsCardTypeStaged(card.CardType);

        public override IResolvable GetRecoveryResolvable(CardInstance card)
        {
            if (!card.Owner.IsProficient(card.Data)) return null;

            var resolvable = new CheckResolvable(
                card,
                card.Owner,
                CardUtils.SkillCheck(9, Skill.Arcane))
            {
                OnSuccess = () => card.Owner.Recharge(card),
                OnFailure = () => card.Owner.Discard(card)
            };

            return CardUtils.CreateDefaultRecoveryResolvable(resolvable, _gameServices);
        }
    }
}
