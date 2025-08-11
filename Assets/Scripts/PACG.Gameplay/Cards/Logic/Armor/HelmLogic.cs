using System;
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class HelmLogic : CardLogicBase
    {
        private readonly ContextManager _contexts;

        private CheckContext Check => _contexts.CheckContext;

        private PlayCardAction GetDamageAction(CardInstance card) => new(card, PF.ActionType.Reveal, ("IsFreely", true), ("Damage", 1));

        public HelmLogic(GameServices gameServices) : base(gameServices) 
        {
            _contexts = gameServices.Contexts;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (CanReveal(card)) actions.Add(GetDamageAction(card));
            return actions;
        }

        bool CanReveal(CardInstance card) => (
            // We can freely reveal for damage if we have a DamageResolvable for the card's owner with Combat damage, or any type of damage if proficient.
            _contexts.CurrentResolvable is DamageResolvable resolvable
            && (resolvable.DamageType == "Combat" || card.Owner.IsProficient(PF.CardType.Armor))
            && resolvable.PlayerCharacter == card.Owner);

        public override void OnStage(CardInstance card, IStagedAction _)
        {
            _contexts.EncounterContext.AddProhibitedTraits(card.Owner, card, "Helm");
        }

        public override void OnUndo(CardInstance card, IStagedAction _)
        {
            _contexts.EncounterContext.UndoProhibitedTraits(card.Owner, card);
        }

    }
}
