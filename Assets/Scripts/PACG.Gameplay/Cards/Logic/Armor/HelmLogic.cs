using System;
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class HelmLogic : CardLogicBase
    {
        private CheckContext Check => GameServices.Contexts.CheckContext;

        private PlayCardAction GetDamageAction(CardInstance card) => new(this, card, PF.ActionType.Reveal, ("IsFreely", true), ("Damage", 1));

        public HelmLogic(GameServices gameServices) : base(gameServices) { }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (CanReveal(card)) actions.Add(GetDamageAction(card));
            return actions;
        }

        bool CanReveal(CardInstance card) => (
            // We can freely reveal for damage if we have a DamageResolvable for the card's owner with Combat damage, or any type of damage if proficient.
            GameServices.Contexts.CurrentResolvable is DamageResolvable resolvable
            && (resolvable.DamageType == "Combat" || card.Owner.IsProficient(PF.CardType.Armor))
            && resolvable.PlayerCharacter == card.Owner);

        public override void OnStage(CardInstance card, IStagedAction _)
        {
            GameServices.Contexts.EncounterContext.AddProhibitedTraits(card.Owner, card, "Helm");
        }

        public override void OnUndo(CardInstance card, IStagedAction _)
        {
            GameServices.Contexts.EncounterContext.UndoProhibitedTraits(card.Owner, card);
        }

    }
}
