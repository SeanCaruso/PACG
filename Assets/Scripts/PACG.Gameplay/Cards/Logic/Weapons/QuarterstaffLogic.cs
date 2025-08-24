using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public class QuarterstaffLogic : CardLogicBase
    {
        // Dependency injections
        private readonly ActionStagingManager _asm;
        private readonly ContextManager _contexts;

        public QuarterstaffLogic(GameServices gameServices) : base(gameServices)
        {
            _asm = gameServices.ASM;
            _contexts = gameServices.Contexts;
        }

        public override CheckModifier GetCheckModifier(IStagedAction action)
        {
            // Reveal to add 1d6; additionally discard to add another 1d6.
            if (action is not PlayCardAction { IsCombat: true }) return null;

            var modifier = new CheckModifier(action.Card)
            {
                RestrictedCategory = CheckCategory.Combat,
                AddedTraits = action.Card.Traits,
                RestrictedSkills = new[] {PF.Skill.Strength, PF.Skill.Melee}.ToList(),
                ProhibitedTraits = new[] { "Offhand" }.ToHashSet(), // After playing, you can't play an Offhand boon this encounter.
                AddedDice = new[] { 6 }.ToList()
            };
            
            if (action.ActionType == PF.ActionType.Discard)
                modifier.AddedDice.Add(6);
            
            return modifier;
        }

        public override void OnCommit(IStagedAction action)
        {
            _contexts.EncounterContext?.AddProhibitedTraits(action.Card.Owner, "Offhand");
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();

            if (IsPlayableForCombat(card))
            {
                // If a weapon hasn't been played yet, display both combat options.
                if (!_contexts.CheckContext.StagedCardTypes.Contains(card.Data.cardType))
                {
                    actions.Add(new PlayCardAction(card, PF.ActionType.Reveal, ("IsCombat", true)));
                    actions.Add(new PlayCardAction(card, PF.ActionType.Discard, ("IsCombat", true)));
                }
                // Otherwise, if this card has already been played, present the discard option only.
                else if (_asm.CardStaged(card))
                {
                    actions.Add(new PlayCardAction(
                        card, PF.ActionType.Discard, ("IsCombat", true), ("IsFreely", true))
                    );
                }
            }
            
            if (CanEvade(card))
            {
                actions.Add(new PlayCardAction(card, PF.ActionType.Discard));
            }

            return actions;
        }

        // Can be played on Strength or Melee combat checks.
        private bool IsPlayableForCombat(CardInstance card) =>
            _contexts.CheckContext is { IsCombatValid: true }
            && _contexts.CheckContext.Character == card.Owner
            && _contexts.CheckContext.CanUseSkill(PF.Skill.Strength, PF.Skill.Melee);

        // Can be played by the owner to evade an Obstacle or Trap barrier.
        private bool CanEvade(CardInstance card) =>
            _contexts.EncounterContext?.CurrentPhase == EncounterPhase.Evasion &&
            _contexts.EncounterContext?.CardData.cardType == PF.CardType.Barrier &&
            _contexts.EncounterContext.HasTrait("Obstacle", "Trap") &&
            _contexts.EncounterContext?.Character == card.Owner;
    }
}
