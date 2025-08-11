using System;
using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public class LongspearLogic : CardLogicBase
    {
        private readonly ContextManager _contexts;

        private CheckContext Check => _contexts.CheckContext;

        private PlayCardAction GetRevealAction(CardInstance card) => new(card, PF.ActionType.Reveal, ("IsCombat", true));
        private PlayCardAction GetRerollAction(CardInstance card) => new(card, PF.ActionType.Discard, ("IsFreely", true));

        public LongspearLogic(GameServices gameServices) : base(gameServices) 
        {
            _contexts = gameServices.Contexts;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (IsCardPlayable(card))
            {
                if (Check.CheckPhase == CheckPhase.PlayCards
                    && !Check.StagedCardTypes.Contains(card.Data.cardType))
                {
                    actions.Add(GetRevealAction(card));
                }

                // We can discard to reroll if we're in the roll dice phase and this card is one of the reroll options.
                if (Check.CheckPhase == CheckPhase.RollDice
                    && ((List<CardInstance>)Check.ContextData.GetValueOrDefault("rerollCards", new List<CardInstance>())).Contains(card))
                {
                    actions.Add(GetRerollAction(card));
                }
            }
            return actions;
        }

        bool IsCardPlayable(CardInstance card) => (
            // All powers on this card are specific to its owner during a Strength or Melee combat check.
            Check != null
            && Check.Resolvable is CombatResolvable resolvable
            && resolvable.Character == card.Owner
            && Check.CanPlayCardWithSkills(PF.Skill.Strength, PF.Skill.Melee));

        public override void OnStage(CardInstance card, IStagedAction action)
        {
            _contexts.EncounterContext.AddProhibitedTraits(card.Owner, card, "Offhand");
            Check.RestrictValidSkills(card, PF.Skill.Strength, PF.Skill.Melee);
        }

        public override void OnUndo(CardInstance card, IStagedAction action)
        {
            _contexts.EncounterContext.UndoProhibitedTraits(card.Owner, card);
            Check.UndoSkillModification(card);
        }

        public override void Execute(CardInstance card, IStagedAction action)
        {
            if (!Check.ContextData.ContainsKey("rerollCards"))
                Check.ContextData["rerollCards"] = new List<CardData>();
            List<CardInstance> rerollSources = (List<CardInstance>)Check.ContextData["rerollCards"];
            
            var revealAction = GetRevealAction(card);
            var rerollAction = GetRerollAction(card);

            // Reveal to use Strength or Melee + 1d8.
            if (action.ActionType == revealAction.ActionType && action.Card == card)
            {
                (PF.Skill skill, int die, int bonus) = card.Owner.GetBestSkill(PF.Skill.Strength, PF.Skill.Melee);
                Check.UsedSkill = skill;
                Check.DicePool.AddDice(1, die, bonus);
                Check.DicePool.AddDice(1, 8);

                rerollSources.Add(card);
            }

            // Discard to reroll.
            if (action.ActionType == rerollAction.ActionType && action.Card == card)
            {
                rerollSources.Remove(card);
                Check.ContextData["doReroll"] = true;
            }
        }
    }
}
