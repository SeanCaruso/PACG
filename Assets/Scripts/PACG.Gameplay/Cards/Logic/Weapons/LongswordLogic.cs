using System;
using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    [PlayableLogicFor("Longsword")]
    public class LongswordLogic : CardLogicBase
    {
        private CheckContext Check => GameServices.Contexts.CheckContext;
        private ActionStagingManager ASM => GameServices.ASM;

        private PlayCardAction _revealAction;
        private PlayCardAction RevealAction => _revealAction ??= new(this, Card, PF.ActionType.Reveal, ("IsCombat", true));

        private PlayCardAction _reloadAction;
        private PlayCardAction ReloadAction => _reloadAction ??= new(this, Card, PF.ActionType.Reload, ("IsCombat", true), ("IsFreely", true));

        private PlayCardAction _revealAndReloadAction;
        private PlayCardAction RevealAndReloadAction => _revealAndReloadAction ??= new(this, Card, PF.ActionType.Reload, ("IsCombat", true));

        public LongswordLogic(GameServices gameServices) : base(gameServices) { }

        protected override List<IStagedAction> GetAvailableCardActions()
        {
            List<IStagedAction> actions = new();
            if (IsCardPlayabe)
            {
                // If a weapon hasn't been played yet, present one or both options.
                if (!Check.StagedCardTypes.Contains(Card.Data.cardType))
                {
                    actions.Add(RevealAction);

                    if (Check.Resolvable is CombatResolvable resolvable && resolvable.Character.IsProficient(PF.CardType.Weapon))
                    {
                        actions.Add(RevealAndReloadAction);
                    }
                }
                // Otherwise, if this card has already been played, present the reload option if proficient.
                else if (ASM.CardStaged(Card) && Check.Resolvable is CombatResolvable res && res.Character.IsProficient(PF.CardType.Weapon))
                {
                    actions.Add(ReloadAction);
                }
            }
            return actions;
        }

        bool IsCardPlayabe => (
            // All powers are specific to the card's owner while playing cards during a Strength or Melee combat check.
            Check != null
            && Check.Resolvable is CombatResolvable resolvable
            && resolvable.Character == Card.Owner
            && Check.CheckPhase == CheckPhase.PlayCards
            && Check.CanPlayCardWithSkills(PF.Skill.Strength, PF.Skill.Melee));

        public override void OnStage(IStagedAction action)
        {
            Check.RestrictValidSkills(Card, PF.Skill.Strength, PF.Skill.Melee);
        }

        public override void OnUndo(IStagedAction action)
        {
            Check.UndoSkillModification(Card);
        }

        public override void Execute(IStagedAction action)
        {
            if (action == RevealAction || action == RevealAndReloadAction)
            {
                // Reveal to use Strength or Melee + 1d8.
                var resolvable = (CombatResolvable)Check.Resolvable;
                (PF.Skill skill, int die, int bonus) = resolvable.Character.GetBestSkill(PF.Skill.Strength, PF.Skill.Melee);
                Check.UsedSkill = skill;
                Check.DicePool.AddDice(1, die, bonus);
                Check.DicePool.AddDice(1, 8);
            }

            // Reload to add another 1d4.
            if (action == ReloadAction || action == RevealAndReloadAction)
            {
                Check.DicePool.AddDice(1, 4);
            }
        }
    }
}
