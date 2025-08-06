using System;
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    [PlayableLogicFor("Helm")]
    public class HelmLogic : CardLogicBase, IPlayableLogic
    {
        private PlayCardAction _damageAction;
        private PlayCardAction DamageAction => _damageAction ??= new(this, Card, PF.ActionType.Reveal, ("IsFreely", true), ("Damage", 1));

        public HelmLogic(ContextManager contextManager, LogicRegistry logicRegistry) : base(contextManager, logicRegistry) { }

        protected override List<IStagedAction> GetAvailableCardActions()
        {
            List<IStagedAction> actions = new();
            if (CanReveal) actions.Add(DamageAction);
            return actions;
        }

        bool CanReveal => (
            // We can freely reveal for damage if we have a DamageResolvable for the card's owner with Combat damage, or any type of damage if proficient.
            Contexts.ResolutionContext?.CurrentResolvable is DamageResolvable resolvable
            && (resolvable.DamageType == "Combat" || Card.Owner.IsProficient(PF.CardType.Armor))
            && resolvable.PlayerCharacter == Card.Owner);

        void IPlayableLogic.OnStage(IStagedAction _)
        {
            Contexts.EncounterContext.AddProhibitedTraits(Card.Owner, Card, "Helm");
        }

        void IPlayableLogic.OnUndo(IStagedAction _)
        {
            Contexts.EncounterContext.UndoProhibitedTraits(Card.Owner, Card);
        }

        void IPlayableLogic.Execute(IStagedAction action) { }
    }
}
