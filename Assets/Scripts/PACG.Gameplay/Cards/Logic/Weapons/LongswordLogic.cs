using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public class LongswordLogic : CardLogicBase
    {
        private readonly ContextManager _contexts;
        private readonly ActionStagingManager _asm;

        private CheckContext Check => _contexts.CheckContext;

        public LongswordLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
            _asm = gameServices.ASM;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (!IsCardPlayable(card)) return actions;

            // If a weapon hasn't been played yet, present one or both options.
            if (!Check.StagedCardTypes.Contains(card.Data.cardType))
            {
                actions.Add(new PlayCardAction(card, PF.ActionType.Reveal, ("IsCombat", true)));

                if (Check.Character.IsProficient(card.Data))
                {
                    actions.Add(new PlayCardAction(card, PF.ActionType.Reload, ("IsCombat", true)));
                }
            }
            // Otherwise, if this card has already been played, present the reload option if proficient.
            else if (_asm.CardStaged(card) && Check.Character.IsProficient(card.Data))
            {
                actions.Add(new PlayCardAction(
                    card,
                    PF.ActionType.Reload,
                    ("IsCombat", true), ("IsFreely", true))
                );
            }

            return actions;
        }

        private bool IsCardPlayable(CardInstance card) =>
            // All powers are specific to the card's owner while playing cards during a Strength or Melee combat check.
            Check is { IsCombatValid: true }
            && Check.Character == card.Owner
            && Check.CanUseSkill(PF.Skill.Strength, PF.Skill.Melee);

        public override void OnStage(CardInstance card, IStagedAction action)
        {
            Check.RestrictCheckCategory(card, CheckCategory.Combat);
            Check.RestrictValidSkills(card, PF.Skill.Strength, PF.Skill.Melee);
            
            Check.AddTraits(card);
        }

        public override void OnUndo(CardInstance card, IStagedAction action)
        {
            Check.UndoCheckRestriction(card);
            Check.UndoSkillModification(card);
            
            Check.RemoveTraits(card);
        }

        public override void Execute(CardInstance card, IStagedAction action, DicePool dicePool)
        {
            if (action is not PlayCardAction playAction) return;

            var isFreely = playAction.ActionData.TryGetValue("IsFreely", out var isFreelyObj) &&
                           isFreelyObj is true;

            // If not freely, Reveal to use Strength or Melee + 1d8.
            if (!isFreely)
                dicePool.AddDice(1, 8);

            // Reload to add another 1d4.
            if (action.ActionType == PF.ActionType.Reload)
                dicePool.AddDice(1, 4);
        }
    }
}
