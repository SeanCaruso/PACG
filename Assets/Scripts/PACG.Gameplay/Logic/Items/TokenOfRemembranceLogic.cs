using System.Collections.Generic;
using System.Linq;
using PACG.Core;
using PACG.Data;
using PACG.SharedAPI;

namespace PACG.Gameplay
{
    public class TokenOfRemembranceLogic : CardLogicBase
    {
        // Dependency injection
        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlow;
        private readonly GameServices _gameServices;
        
        public TokenOfRemembranceLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
            _gameServices = gameServices;
        }

        public override void OnCommit(IStagedAction action)
        {
            var validCards = 
                action.Card.Owner.Discards.Where(c => c.CardType == CardType.Spell).ToList();
            
            if (!validCards.Any()) return;

            var resolvable = new TokenOfRemembranceResolvable(validCards, _gameServices);
            var processor = new NewResolvableProcessor(resolvable, _gameServices);
            _gameFlow.StartPhase(processor, action.Card.ToString());
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();
            
            // Recharge on your check to recharge a spell.
            if (_contexts.TurnContext?.CurrentPhase == TurnPhase.Recovery
                && _contexts.CurrentResolvable is CheckResolvable resolvable
                && resolvable.Card.CardType == CardType.Spell
                && resolvable.Character == card.Owner
                && !resolvable.IsCardTypeStaged(card.CardType))
            {
                var modifier = new CheckModifier(card)
                {
                    AddedDice = new List<int> { 8 }
                };
                actions.Add(new PlayCardAction(card, ActionType.Recharge, modifier));
            }
            
            // Bury to reload a spell from your discards.
            if (_contexts.AreCardsPlayable
                && card.Owner.Discards.Any(c => c.CardType == CardType.Spell))
            {
                actions.Add(new PlayCardAction(card, ActionType.Bury, null));
            }

            return actions;
        }
    }

    public class TokenOfRemembranceResolvable : BaseResolvable
    {
        private readonly IReadOnlyList<CardInstance> _validCards;

        // Dependency injection
        private readonly ActionStagingManager _asm;

        public override bool CancelAbortsPhase => true;

        public TokenOfRemembranceResolvable(List<CardInstance> cards, GameServices gameServices)
        {
            _validCards = cards;

            _asm = gameServices.ASM;
        }

        public override List<IStagedAction> GetAdditionalActionsForCard(CardInstance card)
        {
            // Only one card allowed.
            if (_asm.StagedCards.Count > 0)
                return new List<IStagedAction>();

            List<IStagedAction> actions = new();
            if (_validCards.Contains(card))
                actions.Add(new DefaultAction(card, ActionType.Reload));

            return actions;
        }

        public override bool CanCommit(IReadOnlyList<IStagedAction> actions)
        {
            if (actions.Count == 1)
            {
                GameEvents.SetStatusText("");
                return true;
            }

            GameEvents.SetStatusText("Reload a spell from your discards.");
            return false;
        }
    }
}
