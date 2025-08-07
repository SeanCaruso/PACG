using System;
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    [PlayableLogicFor("Helm")]
    public class HelmLogic : CardLogicBase, IPlayableLogic
    {
        private CheckContext Check => GameServices.Contexts.CheckContext;

        private PlayCardAction _damageAction;
        private PlayCardAction DamageAction => _damageAction ??= new(this, Card, PF.ActionType.Reveal, ("IsFreely", true), ("Damage", 1));

        public HelmLogic(GameServices gameServices) : base(gameServices) { }

        protected override List<IStagedAction> GetAvailableCardActions()
        {
            List<IStagedAction> actions = new();
            if (CanReveal) actions.Add(DamageAction);
            return actions;
        }

        bool CanReveal => (
            // We can freely reveal for damage if we have a DamageResolvable for the card's owner with Combat damage, or any type of damage if proficient.
            GameServices.Contexts.ResolutionContext?.CurrentResolvable is DamageResolvable resolvable
            && (resolvable.DamageType == "Combat" || Card.Owner.IsProficient(PF.CardType.Armor))
            && resolvable.PlayerCharacter == Card.Owner);

        void IPlayableLogic.OnStage(IStagedAction _)
        {
            GameServices.Contexts.EncounterContext.AddProhibitedTraits(Card.Owner, Card, "Helm");
        }

        void IPlayableLogic.OnUndo(IStagedAction _)
        {
            GameServices.Contexts.EncounterContext.UndoProhibitedTraits(Card.Owner, Card);
        }

        void IPlayableLogic.Execute(IStagedAction action) { }
    }
}
