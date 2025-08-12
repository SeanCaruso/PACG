using System;
using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public class LongswordLogic : CardLogicBase
    {
        private readonly ContextManager _contexts;
        private readonly ActionStagingManager _asm;

        private CheckContext Check => _contexts.CheckContext;

        private PlayCardAction GetRevealAction(CardInstance card) => new(card, PF.ActionType.Reveal, ("IsCombat", true));
        private PlayCardAction GetReloadAction(CardInstance card) => new(card, PF.ActionType.Reload, ("IsCombat", true), ("IsFreely", true));
        private PlayCardAction GetRevealAndReloadAction(CardInstance card) => new(card, PF.ActionType.Reload, ("IsCombat", true));

        public LongswordLogic(GameServices gameServices) : base(gameServices) 
        {
            _contexts = gameServices.Contexts;
            _asm = gameServices.ASM;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (IsCardPlayable(card))
            {
                // If a weapon hasn't been played yet, present one or both options.
                if (!Check.StagedCardTypes.Contains(card.Data.cardType))
                {
                    actions.Add(GetRevealAction(card));

                    if (_contexts.CurrentResolvable is CombatResolvable resolvable && resolvable.Character.IsProficient(PF.CardType.Weapon))
                    {
                        actions.Add(GetRevealAndReloadAction(card));
                    }
                }
                // Otherwise, if this card has already been played, present the reload option if proficient.
                else if (_asm.CardStaged(card) && _contexts.CurrentResolvable is CombatResolvable res && res.Character.IsProficient(PF.CardType.Weapon))
                {
                    actions.Add(GetReloadAction(card));
                }
            }
            return actions;
        }

        bool IsCardPlayable(CardInstance card) => (
            // All powers are specific to the card's owner while playing cards during a Strength or Melee combat check.
            Check != null
            && _contexts.CurrentResolvable is CombatResolvable resolvable
            && resolvable.Character == card.Owner
            && Check.CanPlayCardWithSkills(PF.Skill.Strength, PF.Skill.Melee));

        public override void OnStage(CardInstance card, IStagedAction action)
        {
            Check.RestrictValidSkills(card, PF.Skill.Strength, PF.Skill.Melee);
        }

        public override void OnUndo(CardInstance card, IStagedAction action)
        {
            Check.UndoSkillModification(card);
        }

        public override void Execute(IStagedAction action)
        {
            // Always Reveal to use Strength or Melee + 1d8.
            var resolvable = (CombatResolvable)_contexts.CurrentResolvable;
            (PF.Skill skill, int die, int bonus) = resolvable.Character.GetBestSkill(PF.Skill.Strength, PF.Skill.Melee);
            Check.UsedSkill = skill;
            Check.DicePool.AddDice(1, die, bonus);
            Check.DicePool.AddDice(1, 8);

            // Reload to add another 1d4.
            if (action.ActionType == PF.ActionType.Reload)
            {
                Check.DicePool.AddDice(1, 4);
            }
        }
    }
}
