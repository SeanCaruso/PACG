using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public class LongspearLogic : CardLogicBase
    {
        private readonly ContextManager _contexts;

        private CheckContext Check => _contexts.CheckContext;

        public LongspearLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            
            // Can reveal if the owner has a combat check and can use Strength or Melee.
            if (Check is { IsCombatValid: true } &&
                Check.Character == card.Owner &&
                Check.CanUseSkill(PF.Skill.Strength, PF.Skill.Melee) &&
                !Check.StagedCardTypes.Contains(card.Data.cardType))
            {
                actions.Add(new PlayCardAction(card, PF.ActionType.Reveal, ("IsCombat", true)));
            }

            // We can discard to reroll if we're processing a RerollResolvable and this card is one of the reroll options.
            if (_contexts.CurrentResolvable is RerollResolvable
                && ((List<CardLogicBase>)Check.ContextData.GetValueOrDefault("rerollCards", new List<CardLogicBase>()))
                .Contains(this))
            {
                actions.Add(new PlayCardAction(card, PF.ActionType.Discard, ("IsFreely", true)));
            }

            return actions;
        }

        public override void OnStage(CardInstance card, IStagedAction action)
        {
            _contexts.EncounterContext.AddProhibitedTraits(card.Owner, card, "Offhand");
            
            Check?.RestrictCheckCategory(card, CheckCategory.Combat);
            Check?.RestrictValidSkills(card, PF.Skill.Strength, PF.Skill.Melee);
            Check?.AddTraits(card);
        }

        public override void OnUndo(CardInstance card, IStagedAction action)
        {
            _contexts.EncounterContext.UndoProhibitedTraits(card.Owner, card);
            
            Check?.UndoCheckRestriction(card);
            Check?.UndoSkillModification(card);
            Check?.RemoveTraits(card);
        }

        public override void Execute(CardInstance card, IStagedAction action, DicePool dicePool)
        {
            if (!Check.ContextData.ContainsKey("rerollCards"))
                Check.ContextData["rerollCards"] = new List<CardLogicBase>();
            var rerollSources = (List<CardLogicBase>)Check.ContextData["rerollCards"];

            switch (action.ActionType)
            {
                // Reveal to use Strength or Melee + 1d8.
                case PF.ActionType.Reveal:
                    dicePool.AddDice(1, 8);
                    rerollSources.Add(this);
                    break;
                // Discard to reroll.
                case PF.ActionType.Discard:
                    rerollSources.Remove(this);
                    Check.ContextData["doReroll"] = true;
                    break;
            }
        }
    }
}
