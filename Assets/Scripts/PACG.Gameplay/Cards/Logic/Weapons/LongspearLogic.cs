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
                if (_contexts.CurrentResolvable is CombatResolvable
                    && !Check.StagedCardTypes.Contains(card.Data.cardType))
                {
                    actions.Add(GetRevealAction(card));
                }

                // We can discard to reroll if we're processing a RerollResolvable and this card is one of the reroll options.
                if (_contexts.CurrentResolvable is RerollResolvable
                    && ((List<CardLogicBase>)Check.ContextData.GetValueOrDefault("rerollCards", new List<CardLogicBase>())).Contains(this))
                {
                    actions.Add(GetRerollAction(card));
                }
            }
            return actions;
        }

        bool IsCardPlayable(CardInstance card) => (
            // All powers on this card are specific to its owner during a Strength or Melee combat check.
            Check != null
            && _contexts.CurrentResolvable is CombatResolvable resolvable
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
                Check.ContextData["rerollCards"] = new List<CardLogicBase>();
            List<CardLogicBase> rerollSources = (List<CardLogicBase>)Check.ContextData["rerollCards"];

            // Reveal to use Strength or Melee + 1d8.
            if (action.ActionType == PF.ActionType.Reveal)
            {
                var resolvable = _contexts.CurrentResolvable as CombatResolvable;
                (PF.Skill skill, int die, int bonus) = resolvable.Character.GetBestSkill(PF.Skill.Strength, PF.Skill.Melee);
                Check.UsedSkill = skill;
                Check.DicePool.AddDice(1, die, bonus);
                Check.DicePool.AddDice(1, 8);

                rerollSources.Add(this);
            }

            // Discard to reroll.
            if (action.ActionType == PF.ActionType.Discard)
            {
                rerollSources.Remove(this);
                Check.ContextData["doReroll"] = true;
            }
        }
    }
}
