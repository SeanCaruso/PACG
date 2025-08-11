using System;
using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public class HalfPlateLogic : CardLogicBase
    {
        private CheckContext Check => GameServices.Contexts.CheckContext;
        private ActionStagingManager ASM => GameServices.ASM;

        private PlayCardAction GetDisplayAction(CardInstance card) => new(this, card, PF.ActionType.Display);
        private PlayCardAction GetDrawAction(CardInstance card) => new(this, card, PF.ActionType.Draw, ("Damage", 2));
        private PlayCardAction GetBuryAction(CardInstance card) => new(this, card, PF.ActionType.Bury, ("ReduceDamageTo", 0));

        public HalfPlateLogic(GameServices gameServices) : base(gameServices) { }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (CanDisplay(card)) actions.Add(GetDisplayAction(card));
            if (CanDraw(card)) actions.Add(GetDrawAction(card));
            if (CanBury(card)) actions.Add(GetBuryAction(card));
            return actions;
        }

        bool CanDisplay(CardInstance card) => (
            // We can display if not currently displayed and we haven't played an Armor during a check.
            !card.Owner.DisplayedCards.Contains(card)
            && (Check == null || !Check.StagedCardTypes.Contains(PF.CardType.Armor)));

        bool CanDraw(CardInstance card) => (
            // We can draw for damage if displayed and we have a DamageResolvable for the card's owner with Combat damage.
            Check != null
            && card.Owner.DisplayedCards.Contains(card)
            && (ASM.CardStaged(card) || !Check.StagedCardTypes.Contains(PF.CardType.Armor)) // If we staged the Display this check, we can freely draw.
            && GameServices.Contexts.CurrentResolvable is DamageResolvable resolvable
            && resolvable.DamageType == "Combat"
            && resolvable.PlayerCharacter == card.Owner);

        bool CanBury(CardInstance card) => (
            // We can bury for damage if displayed, the owner is proficient, and we have a DamageResolvable for the card's owner.
            Check != null
            && card.Owner.DisplayedCards.Contains(card)
            && (ASM.CardStaged(card) || !Check.StagedCardTypes.Contains(PF.CardType.Armor)) // If we staged the Display this check, we can freely bury.
            && card.Owner.IsProficient(PF.CardType.Armor)
            && GameServices.Contexts.CurrentResolvable is DamageResolvable resolvable
            && resolvable.PlayerCharacter == card.Owner);
    }
}
