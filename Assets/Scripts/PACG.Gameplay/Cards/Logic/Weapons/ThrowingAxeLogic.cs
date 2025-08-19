using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public class ThrowingAxeLogic : CardLogicBase
    {
        private readonly ContextManager _contexts;

        private CheckContext Check => _contexts.CheckContext;

        public ThrowingAxeLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (CanReveal(card))
                actions.Add(new PlayCardAction(card, PF.ActionType.Reveal, ("IsCombat", true)));
            if (CanDiscard(card))
                actions.Add(new PlayCardAction(card, PF.ActionType.Discard, ("IsCombat", true), ("IsFreely", true)));
            return actions;
        }

        private readonly PF.Skill[] _validSkills =
            { PF.Skill.Strength, PF.Skill.Dexterity, PF.Skill.Melee, PF.Skill.Ranged };

        private bool CanReveal(CardInstance card) => (
            // Reveal power can be used by the current owner while playing cards for a Strength, Dexterity, Melee, or Ranged combat check.
            Check != null
            && _contexts.CurrentResolvable is CombatResolvable resolvable
            && resolvable.Character == card.Owner
            && !Check.StagedCardTypes.Contains(card.Data.cardType)
            && Check.CanUseSkill(_validSkills));

        private bool CanDiscard(CardInstance card) =>
            // Discard power can be freely used on a local combat check while playing cards if the owner is proficient.
            Check != null
            && card.Owner.IsProficient(card.Data)
            && _contexts.CurrentResolvable is CombatResolvable
            && Check.IsLocal(card.Owner);

        public override void OnStage(CardInstance card, IStagedAction action)
        {
            Check.RestrictValidSkills(card, _validSkills);
        }

        public override void OnUndo(CardInstance card, IStagedAction action)
        {
            Check.UndoSkillModification(card);
        }

        public override void Execute(CardInstance card, IStagedAction action)
        {
            switch (action.ActionType)
            {
                // Reveal to use Strength, Dexterity, Melee, or Ranged + 1d8.       
                case PF.ActionType.Reveal:
                {
                    if (_contexts.CurrentResolvable is not CombatResolvable resolvable) return;
                    var (skill, die, bonus) = resolvable.Character.GetBestSkill(
                        PF.Skill.Strength, PF.Skill.Dexterity, PF.Skill.Melee, PF.Skill.Ranged
                    );
                    Check.UsedSkill = skill;
                    Check.DicePool.AddDice(1, die, bonus);
                    Check.DicePool.AddDice(1, 8);
                    break;
                }
                // Discard to add 1d6.
                case PF.ActionType.Discard:
                    Check.DicePool.AddDice(1, 6);
                    break;
            }
        }
    }
}
