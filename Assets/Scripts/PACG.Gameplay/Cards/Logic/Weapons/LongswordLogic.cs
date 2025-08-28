using System.Collections.Generic;
using System.Linq;
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

        public override CheckModifier GetCheckModifier(IStagedAction action)
        {
            if (action is not PlayCardAction playAction) return null;

            // All powers are specific to using this card for a Strength or Melee combat check.
            var modifier = new CheckModifier(action.Card)
            {
                RestrictedCategory = CheckCategory.Combat,
                RestrictedSkills = new[] { Skill.Strength, Skill.Melee }.ToList(),
                AddedTraits = action.Card.Traits
            };

            var isFreely = playAction.ActionData.TryGetValue("IsFreely", out var isFreelyObj) && isFreelyObj is true;

            // If not freely, Reveal to use Strength or Melee + 1d8.
            if (!isFreely)
                modifier.AddedDice.Add(8);

            // Reload to add another 1d4.
            if (action.ActionType == ActionType.Reload)
                modifier.AddedDice.Add(4);

            return modifier;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (!IsCardPlayable(card)) return actions;

            // If a weapon hasn't been played yet, present one or both options.
            if (!Check.StagedCardTypes.Contains(card.Data.cardType))
            {
                actions.Add(new PlayCardAction(card, ActionType.Reveal, ("IsCombat", true)));

                if (Check.Character.IsProficient(card.Data))
                {
                    actions.Add(new PlayCardAction(card, ActionType.Reload, ("IsCombat", true)));
                }
            }
            // Otherwise, if this card has already been played, present the reload option if proficient.
            else if (_asm.CardStaged(card) && Check.Character.IsProficient(card.Data))
            {
                actions.Add(new PlayCardAction(
                    card,
                    ActionType.Reload,
                    ("IsCombat", true), ("IsFreely", true))
                );
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
