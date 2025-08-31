using System.Collections.Generic;
using PACG.Core;
using PACG.Data;

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
                if (!_contexts.CurrentResolvable.IsCardTypeStaged(card.CardType))
                {
                    var revealModifier = new CheckModifier(card)
                    {
                        RestrictedCategory = CheckCategory.Combat,
                        AddedTraits = card.Traits,
                        RestrictedSkills = new List<Skill> { Skill.Strength, Skill.Melee },
                        ProhibitedTraits = new HashSet<string>
                            { "Offhand" }, // After playing, you can't play an Offhand boon this encounter.
                        AddedDice = new List<int> { 6 }
                    };

                    actions.Add(new PlayCardAction(card, ActionType.Reveal, revealModifier, ("IsCombat", true)));

                    var revealAndDiscardModifier = new CheckModifier(card)
                    {
                        RestrictedCategory = CheckCategory.Combat,
                        AddedTraits = card.Traits,
                        RestrictedSkills = new List<Skill> { Skill.Strength, Skill.Melee },
                        ProhibitedTraits = new HashSet<string>
                            { "Offhand" }, // After playing, you can't play an Offhand boon this encounter.
                        AddedDice = new List<int> { 6, 6 }
                    };

                    actions.Add(new PlayCardAction(card, ActionType.Discard, revealAndDiscardModifier,
                        ("IsCombat", true)));
                }
                // Otherwise, if this card has already been played, present the discard option only.
                else if (_asm.CardStaged(card))
                {
                    var discardModifier = new CheckModifier(card)
                    {
                        RestrictedCategory = CheckCategory.Combat,
                        RestrictedSkills = new List<Skill> { Skill.Strength, Skill.Melee },
                        ProhibitedTraits = new HashSet<string>
                            { "Offhand" }, // After playing, you can't play an Offhand boon this encounter.
                        AddedDice = new List<int> { 6 }
                    };

                    actions.Add(new PlayCardAction(
                        card, ActionType.Discard, discardModifier, ("IsCombat", true), ("IsFreely", true))
                    );
                }
            }

            if (CanDiscardToEvade(card))
            {
                actions.Add(new PlayCardAction(card, ActionType.Discard, null));
            }

            return actions;
        }

        // Can be played on Strength or Melee combat checks.
        private bool IsPlayableForCombat(CardInstance card) =>
            _contexts.CheckContext is { IsCombatValid: true }
            && _contexts.CheckContext.Character == card.Owner
            && _contexts.CheckContext.CanUseSkill(Skill.Strength, Skill.Melee);

        // Can be played by the owner to evade an Obstacle or Trap barrier.
        private bool CanDiscardToEvade(CardInstance card) =>
            _contexts.EncounterContext?.CurrentPhase == EncounterPhase.Evasion
            && _contexts.EncounterContext?.CardData.cardType == CardType.Barrier
            && _contexts.EncounterContext.HasTrait("Obstacle", "Trap")
            && _contexts.EncounterContext?.Character == card.Owner;
    }
}
