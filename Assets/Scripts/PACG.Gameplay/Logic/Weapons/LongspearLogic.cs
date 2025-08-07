using System;
using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    [PlayableLogicFor("Longspear")]
    public class LongspearLogic : CardLogicBase, IPlayableLogic
    {
        private CheckContext Check => GameServices.Contexts.CheckContext;

        private PlayCardAction _revealAction;
        private PlayCardAction RevealAction => _revealAction ??= new(this, Card, PF.ActionType.Reveal, ("IsCombat", true));

        private PlayCardAction _rerollAction;
        private PlayCardAction RerollAction => _rerollAction ??= new(this, Card, PF.ActionType.Discard, ("IsFreely", true));

        public LongspearLogic(GameServices gameServices) : base(gameServices) { }

        protected override List<IStagedAction> GetAvailableCardActions()
        {
            List<IStagedAction> actions = new();
            if (IsCardPlayable)
            {
                if (Check.CheckPhase == CheckPhase.PlayCards
                    && !Check.StagedCardTypes.Contains(Card.Data.cardType))
                {
                    actions.Add(RevealAction);
                }

                // We can discard to reroll if we're in the roll dice phase and this card is one of the reroll options.
                if (Check.CheckPhase == CheckPhase.RollDice
                    && ((List<CardInstance>)Check.ContextData.GetValueOrDefault("rerollCards", new List<CardInstance>())).Contains(Card))
                {
                    actions.Add(RerollAction);
                }
            }
            return actions;
        }

        bool IsCardPlayable => (
            // All powers on this card are specific to its owner during a Strength or Melee combat check.
            Check != null
            && Check.Resolvable is CombatResolvable resolvable
            && resolvable.Character == Card.Owner
            && Check.CanPlayCardWithSkills(PF.Skill.Strength, PF.Skill.Melee));

        void IPlayableLogic.OnStage(IStagedAction action)
        {
            GameServices.Contexts.EncounterContext.AddProhibitedTraits(Card.Owner, Card, "Offhand");
            Check.RestrictValidSkills(Card, PF.Skill.Strength, PF.Skill.Melee);
        }

        void IPlayableLogic.OnUndo(IStagedAction action)
        {
            GameServices.Contexts.EncounterContext.UndoProhibitedTraits(Card.Owner, Card);
            Check.UndoSkillModification(Card);
        }

        void IPlayableLogic.Execute(IStagedAction action)
        {
            if (!Check.ContextData.ContainsKey("rerollCards"))
                Check.ContextData["rerollCards"] = new List<CardData>();
            List<CardInstance> rerollSources = (List<CardInstance>)Check.ContextData["rerollCards"];

            // Reveal to use Strength or Melee + 1d8.
            if (action == RevealAction)
            {
                (PF.Skill skill, int die, int bonus) = Card.Owner.GetBestSkill(PF.Skill.Strength, PF.Skill.Melee);
                Check.UsedSkill = skill;
                Check.DicePool.AddDice(1, die, bonus);
                Check.DicePool.AddDice(1, 8);

                rerollSources.Add(Card);
            }

            // Discard to reroll.
            if (action == RerollAction)
            {
                rerollSources.Remove(Card);
                Check.ContextData["doReroll"] = true;
            }
        }
    }
}
