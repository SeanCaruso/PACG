using System;
using System.Collections.Generic;
using System.Linq;
using PACG.SharedAPI;

namespace PACG.Gameplay
{
    public class CheckResolvable : BaseResolvable
    {
        public ICard Card { get; }
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
        }

        public override bool CanCommit(List<IStagedAction> actions) => true;

        public override IProcessor CreateProcessor(GameServices gameServices) =>
            new CheckController(this, gameServices);

        public override StagedActionsState GetUIState(List<IStagedAction> actions)
        {
            // The only option is Committing.
            return new StagedActionsState
            {
                IsCommitButtonVisible = true
            };
        }
    }
}
