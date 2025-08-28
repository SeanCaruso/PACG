using System.Collections.Generic;

namespace PACG.Gameplay
{
    public class HorseLogic : CardLogicBase
    {
        // Dependency injection of services
        private readonly ActionStagingManager _asm;
        private readonly ContextManager _contexts;
        
        public HorseLogic(GameServices gameServices) : base(gameServices)
        {
            _asm = gameServices.ASM;
            _contexts = gameServices.Contexts;
        }

        public override void OnCommit(IStagedAction action)
        {
            // Discard to explore with +1d4 on the first check.
            if (action.ActionType != PF.ActionType.Discard) return;
            
            _contexts.TurnContext.AddExploreEffect(new SkillBonusExploreEffect(
                1,
                4,
                0,
                true)
            );
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();
            
            // Recharge to move while not in an encounter.
            if (_contexts.CurrentResolvable == null
                && _contexts.EncounterContext == null
                && _asm.StagedCards.Count == 0)
            {
                actions.Add(new MoveAction(card, PF.ActionType.Recharge));
            }

            // Discard to explore.
            if (_contexts.IsExplorePossible && card.Owner == _contexts.TurnContext.Character)
                actions.Add(new ExploreAction(card, PF.ActionType.Discard));
            
            return actions;
        }
    }
}
