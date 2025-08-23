
using System.Collections.Generic;
using System.Linq;
using PACG.Core;

namespace PACG.Gameplay
{
    public class ClockworkServantLogic : CardLogicBase
    {
        // Dependency injection of services
        private readonly ContextManager _contexts;
        
        public ClockworkServantLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        public override void OnStage(CardInstance card, IStagedAction action)
        {
            if (action.ActionType == PF.ActionType.Recharge)
                _contexts.CheckContext.RestrictValidSkills(card, PF.Skill.Intelligence, PF.Skill.Craft);
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
                    // Recharge for +1d6 on a local Intelligence or Craft check.
                    dicePool.AddDice(1, 6);
                    break;
                case PF.ActionType.Bury:
                case PF.ActionType.Banish:
                    // Bury or Banish to explore
                    _contexts.TurnContext.AddExploreEffect(new BasicExploreEffect());
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
                actions.Add(new ExploreAction(card, PF.ActionType.Bury));
                actions.Add(new ExploreAction(card, PF.ActionType.Banish));
            }

            return actions;
        }

        public override IResolvable GetRecoveryResolvable(CardInstance card)
        {
            // Craft 8 to recharge.
            return new CheckResolvable(card, card.Owner, CardUtils.SkillCheck(8, PF.Skill.Craft));
        }

        // Can recharge on a local Intelligence or Craft check.
        private bool CanRecharge(CardInstance card) => 
            _contexts.CheckContext != null &&
            _contexts.CheckContext.IsLocal(card.Owner) &&
            !_contexts.CheckContext.StagedCardTypes.Contains(card.Data.cardType) &&
            _contexts.CheckContext.CanUseSkill(PF.Skill.Intelligence, PF.Skill.Craft);
    }
}
