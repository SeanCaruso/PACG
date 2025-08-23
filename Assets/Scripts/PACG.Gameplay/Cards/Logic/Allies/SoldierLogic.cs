
using System.Collections.Generic;
using System.Linq;
using PACG.Core;

namespace PACG.Gameplay
{
    public class SoldierLogic : CardLogicBase
    {
        // Dependency injection of services
        private readonly ContextManager _contexts;
        
        public SoldierLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        public override void OnStage(CardInstance card, IStagedAction action)
        {
            if (action.ActionType == PF.ActionType.Recharge)
                _contexts.CheckContext.RestrictValidSkills(card, PF.Skill.Strength, PF.Skill.Melee);
        }

        public override void OnUndo(CardInstance card, IStagedAction action)
        {
            if (action.ActionType == PF.ActionType.Recharge)
                _contexts.CheckContext.UndoSkillModification(card);
        }

        public override void Execute(CardInstance card, IStagedAction action, DicePool dicePool)
        {
            switch (action.ActionType)
            {
                case PF.ActionType.Recharge when dicePool != null:
                    // Recharge for +1d4 on a local Strength or Melee check.
                    dicePool.AddDice(1, 4);
                    break;
                case PF.ActionType.Discard:
                    // Discard to explore - +1d4 on Strength and Melee checks.
                    _contexts.TurnContext.AddExploreEffect(new SkillBonusExploreEffect(
                        1,
                        4,
                        0,
                        false,
                        PF.Skill.Strength, PF.Skill.Melee)
                    );
                    break;
            }
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();
            
            if (CanRecharge(card))
                actions.Add(new PlayCardAction(card, PF.ActionType.Recharge));

            if (_contexts.IsExplorePossible && card.Owner == _contexts.TurnContext.Character)
            {
                actions.Add(new ExploreAction(card, PF.ActionType.Discard));
            }

            return actions;
        }

        // Can recharge on a local Strength or Melee check.
        private bool CanRecharge(CardInstance card) => (
            _contexts.CheckContext != null &&
            _contexts.CurrentResolvable is CheckResolvable &&
            _contexts.CheckContext.IsLocal(card.Owner) &&
            !_contexts.CheckContext.StagedCardTypes.Contains(PF.CardType.Ally) &&
            _contexts.CheckContext.CanUseSkill(PF.Skill.Strength, PF.Skill.Melee)
        );
    }
}
