using System;
using System.Collections.Generic;
using System.Linq;
using PACG.Core;
using PACG.SharedAPI;

namespace PACG.Gameplay
{
    public enum CheckVerb
    {
        Defeat,
        Acquire,
        Close,
        Recover
    }
    
    public class CheckResolvable : BaseResolvable
    {
        public ICard Card { get; }
        public CheckVerb Verb { get; set; }
        public PlayerCharacter Character { get; }
        public List<CheckStep> CheckSteps { get; }
        public bool HasCombat => CheckSteps.Any(step => step.category == CheckCategory.Combat);
        public bool HasSkill => CheckSteps.Any(step => step.category == CheckCategory.Skill);
        public Action OnSuccess { get; set; } = () => { };
        public Action OnFailure { get; set; } = () => { };

        public CheckResolvable(ICard card, PlayerCharacter character, CheckRequirement checkRequirement)
        {
            Card = card;
            Character = character;
            CheckSteps = checkRequirement.checkSteps.ToList();
            
            // Default to defeat for banes, acquire for boons.
            Verb = PF.IsBoon(Card.CardType) ? CheckVerb.Acquire : CheckVerb.Defeat;
        }

        public override bool CanCommit(IReadOnlyList<IStagedAction> actions) => true;

        public override IProcessor CreateProcessor(GameServices gameServices) =>
            new CheckController(this, gameServices);

        public override StagedActionsState GetUIState(IReadOnlyList<IStagedAction> actions)
        {
            // The only option is Committing.
            return new StagedActionsState
            {
                IsCancelButtonVisible = actions.Count > 0,
                IsCommitButtonVisible = true
            };
        }
    }
}
