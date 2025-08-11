using System;
using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public class LongswordLogic : CardLogicBase
    {
        private CheckContext Check => GameServices.Contexts.CheckContext;
        private ActionStagingManager ASM => GameServices.ASM;

        private PlayCardAction GetRevealAction(CardInstance card) => new(this, card, PF.ActionType.Reveal, ("IsCombat", true));
        private PlayCardAction GetReloadAction(CardInstance card) => new(this, card, PF.ActionType.Reload, ("IsCombat", true), ("IsFreely", true));
        private PlayCardAction GetRevealAndReloadAction(CardInstance card) => new(this, card, PF.ActionType.Reload, ("IsCombat", true));

        public LongswordLogic(GameServices gameServices) : base(gameServices) { }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (IsCardPlayabe(card))
            {
                // If a weapon hasn't been played yet, present one or both options.
                if (!Check.StagedCardTypes.Contains(card.Data.cardType))
                {
                    actions.Add(GetRevealAction(card));

                    if (Check.Resolvable is CombatResolvable resolvable && resolvable.Character.IsProficient(PF.CardType.Weapon))
                    {
                        actions.Add(GetRevealAndReloadAction(card));
                    }
                }
                // Otherwise, if this card has already been played, present the reload option if proficient.
                else if (ASM.CardStaged(card) && Check.Resolvable is CombatResolvable res && res.Character.IsProficient(PF.CardType.Weapon))
                {
                    actions.Add(GetReloadAction(card));
                }
            }
            return actions;
        }

        bool IsCardPlayabe(CardInstance card) => (
            // All powers are specific to the card's owner while playing cards during a Strength or Melee combat check.
            Check != null
            && Check.Resolvable is CombatResolvable resolvable
            && resolvable.Character == card.Owner
            && Check.CheckPhase == CheckPhase.PlayCards
            && Check.CanPlayCardWithSkills(PF.Skill.Strength, PF.Skill.Melee));

        public override void OnStage(CardInstance card, IStagedAction action)
        {
            Check.RestrictValidSkills(card, PF.Skill.Strength, PF.Skill.Melee);
        }

        public override void OnUndo(CardInstance card, IStagedAction action)
        {
            Check.UndoSkillModification(card);
        }

        public override void Execute(CardInstance card, IStagedAction action)
        {
            var revealAction = GetRevealAction(card);
            var reloadAction = GetReloadAction(card);
            var revealAndReloadAction = GetRevealAndReloadAction(card);
            
            if ((action.ActionType == revealAction.ActionType && action.Card == card) || 
                (action.ActionType == revealAndReloadAction.ActionType && action.Card == card))
            {
                // Reveal to use Strength or Melee + 1d8.
                var resolvable = (CombatResolvable)Check.Resolvable;
                (PF.Skill skill, int die, int bonus) = resolvable.Character.GetBestSkill(PF.Skill.Strength, PF.Skill.Melee);
                Check.UsedSkill = skill;
                Check.DicePool.AddDice(1, die, bonus);
                Check.DicePool.AddDice(1, 8);
            }

            // Reload to add another 1d4.
            if ((action.ActionType == reloadAction.ActionType && action.Card == card) || 
                (action.ActionType == revealAndReloadAction.ActionType && action.Card == card))
            {
                Check.DicePool.AddDice(1, 4);
            }
        }
    }
}
