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

        public override void OnStage(CardInstance card, IStagedAction action)
        {
            _contexts.EncounterContext?.AddProhibitedTraits(card.Owner, card, "Offhand");
        }

        public override void OnUndo(CardInstance card, IStagedAction action)
        {
            _contexts.EncounterContext?.UndoProhibitedTraits(card.Owner, card);
        }

        public override void Execute(CardInstance card, IStagedAction action)
        {
            // Only handle combat powers. Evasion is handled by the Evasion Processor.
            if (_contexts.CurrentResolvable is not CombatResolvable resolvable ||
                _contexts.CheckContext == null)
            {
                return;
            }

            var (skill, die, bonus) = resolvable.Character.GetBestSkill(PF.Skill.Strength, PF.Skill.Melee);
            _contexts.CheckContext.UsedSkill = skill;
            _contexts.CheckContext.DicePool.AddDice(1, die, bonus);
            
            // Reveal to add 1d6; additionally discard to add another 1d6.
            _contexts.CheckContext.DicePool.AddDice(action.ActionType == PF.ActionType.Reveal ? 1 : 2, 6);

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
            _contexts.CheckContext != null
            && _contexts.CurrentResolvable is CombatResolvable resolvable
            && resolvable.Character == card.Owner
            && _contexts.CheckContext.CanUseSkill(PF.Skill.Strength, PF.Skill.Melee);

        // Can be played by the owner to evade an Obstacle or Trap barrier.
        private bool CanEvade(CardInstance card) =>
            _contexts.EncounterContext?.CurrentPhase == EncounterPhase.Evasion &&
            _contexts.EncounterContext?.CardData.cardType == PF.CardType.Barrier &&
            _contexts.EncounterContext.HasTrait("Obstacle", "Trap") &&
            _contexts.EncounterContext?.Character == card.Owner;
    }
}
