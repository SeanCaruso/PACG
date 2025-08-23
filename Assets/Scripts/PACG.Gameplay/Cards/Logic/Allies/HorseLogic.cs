using System.Collections.Generic;
using PACG.Core;

namespace PACG.Gameplay
{
    public class HorseLogic : CardLogicBase
    {
        // Dependency injection of services
        private readonly ContextManager _contexts;
        
        public HorseLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        public override void Execute(CardInstance card, IStagedAction action, DicePool dicePool)
        {
            switch (action.ActionType)
            {
                // Discard to explore with +1d4 on the first check.
                case PF.ActionType.Discard:
                    _contexts.TurnContext.AddExploreEffect(new SkillBonusExploreEffect(
                        1,
                        4,
                        0,
                        true)
                    );
                    break;
            }
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();
            
            // Recharge to move while not in an encounter.
            if (_contexts.CurrentResolvable == null && _contexts.EncounterContext == null)
                actions.Add(new MoveAction(card, PF.ActionType.Recharge));
            
            // Discard to explore.
            if (_contexts.IsExplorePossible && card.Owner == _contexts.TurnContext.Character)
                actions.Add(new ExploreAction(card, PF.ActionType.Discard));
            
            return actions;
        }
    }
}
