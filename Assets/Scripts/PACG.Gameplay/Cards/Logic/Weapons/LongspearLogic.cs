using System.Collections.Generic;
using System.Linq;
using PACG.Core;

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

        public override CheckModifier GetCheckModifier(IStagedAction action)
        {
            // Reveal to use Strength or Melee + 1d8.
            if (action.ActionType != ActionType.Reveal) return null;

            return new CheckModifier(action.Card)
            {
                RestrictedCategory = CheckCategory.Combat,
                AddedTraits = action.Card.Traits,
                RestrictedSkills = new List<Skill> { Skill.Strength, Skill.Melee },
                ProhibitedTraits = new HashSet<string> { "Offhand" },
                AddedDice = new List<int> { 8 }
                
            };
        }

        public override void OnCommit(IStagedAction action)
        {
            _contexts.EncounterContext?.AddProhibitedTraits(action.Card.Owner, "Offhand");

            if (!Check.ContextData.ContainsKey("rerollCards"))
                Check.ContextData["rerollCards"] = new List<CardLogicBase>();
            var rerollSources = (List<CardLogicBase>)Check.ContextData["rerollCards"];

            switch (action.ActionType)
            {
                case ActionType.Reveal:
                    rerollSources.Add(this);
                    break;
                case ActionType.Discard:
                    rerollSources.Remove(this);
                    Check.ContextData["doReroll"] = true;
                    break;
            }
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();

            // Can reveal if the owner has a combat check and can use Strength or Melee.
            if (Check is { IsCombatValid: true }
                && _contexts.CurrentResolvable is CheckResolvable {HasCombat: true}
                && Check.Character == card.Owner
                && Check.CanUseSkill(Skill.Strength, Skill.Melee)
                && !Check.StagedCardTypes.Contains(card.Data.cardType))
            {
                actions.Add(new PlayCardAction(card, ActionType.Reveal, ("IsCombat", true)));
            }

            // We can discard to reroll if we're processing a RerollResolvable and this card is one of the reroll options.
            if (_contexts.CurrentResolvable is RerollResolvable
                && ((List<CardLogicBase>)Check.ContextData.GetValueOrDefault("rerollCards", new List<CardLogicBase>()))
                .Contains(this))
            {
                actions.Add(new PlayCardAction(card, ActionType.Discard, ("IsFreely", true)));
            }

            return actions;
        }
    }
}
