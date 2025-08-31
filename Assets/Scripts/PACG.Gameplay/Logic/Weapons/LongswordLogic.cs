using System.Collections.Generic;
using PACG.Core;

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
            if (_contexts.CurrentResolvable.CanStageType(card.CardType))
            {
                var revealModifier = new CheckModifier(card)
                {
                    RestrictedCategory = CheckCategory.Combat,
                    RestrictedSkills = new List<Skill> { Skill.Strength, Skill.Melee },
                    AddedTraits = card.Traits,
                    AddedDice = new List<int> { 8 }
                };
                actions.Add(new PlayCardAction(card, ActionType.Reveal, revealModifier, ("IsCombat", true)));

                if (!Check.Character.IsProficient(card.Data)) return actions;
                
                var revealAndReloadModifier = new CheckModifier(card)
                {
                    RestrictedCategory = CheckCategory.Combat,
                    RestrictedSkills = new List<Skill> { Skill.Strength, Skill.Melee },
                    AddedTraits = card.Traits,
                    AddedDice = new List<int> { 8, 4 }
                };
                actions.Add(new PlayCardAction(card, ActionType.Reload, revealAndReloadModifier, ("IsCombat", true)));
            }
            // Otherwise, if this card has already been played, present the reload option if proficient.
            else if (_asm.CardStaged(card) && Check.Character.IsProficient(card.Data))
            {
                var reloadModifier = new CheckModifier(card)
                {
                    RestrictedCategory = CheckCategory.Combat,
                    RestrictedSkills = new List<Skill> { Skill.Strength, Skill.Melee },
                    AddedDice = new List<int> { 4 }
                };
                actions.Add(new PlayCardAction(
                    card,
                    ActionType.Reload,
                    reloadModifier,
                    ("IsCombat", true), ("IsFreely", true)));
            }

            return actions;
        }

        private bool IsCardPlayable(CardInstance card) =>
            // All powers are specific to the card's owner while playing cards during a Strength or Melee combat check.
            Check is { IsCombatValid: true }
            && _contexts.CurrentResolvable is CheckResolvable { HasCombat: true }
            && Check.Character == card.Owner
            && Check.CanUseSkill(Skill.Strength, Skill.Melee);
    }
}
