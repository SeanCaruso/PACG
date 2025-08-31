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

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();
            
            var modifier = new CheckModifier(card)
            {
                AddedDice = new List<int> { 6 }
            };

            if (CanBanishOnCheck(card) || CanBanishToEvade(card))
                actions.Add(new PlayCardAction(card, ActionType.Banish, modifier));

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
