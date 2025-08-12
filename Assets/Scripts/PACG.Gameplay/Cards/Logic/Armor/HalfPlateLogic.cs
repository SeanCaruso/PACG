using System;
using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public class HalfPlateLogic : CardLogicBase
    {
        private readonly ContextManager _contexts;
        private readonly ActionStagingManager _asm;

        private CheckContext Check => _contexts.CheckContext;

        private PlayCardAction GetDisplayAction(CardInstance card) => new(card, PF.ActionType.Display);
        private PlayCardAction GetDrawAction(CardInstance card) => new(card, PF.ActionType.Draw, ("Damage", 2));
        private PlayCardAction GetFreelyDrawAction(CardInstance card) => new(card, PF.ActionType.Draw, ("Damage", 2), ("IsFreely", true));
        private PlayCardAction GetBuryAction(CardInstance card) => new(card, PF.ActionType.Bury, ("ReduceDamageTo", 0));

        public HalfPlateLogic(GameServices gameServices) : base(gameServices) 
        {
            _contexts = gameServices.Contexts;
            _asm = gameServices.ASM;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (CanDisplay(card)) actions.Add(GetDisplayAction(card));
            if (CanDraw(card)) actions.Add(GetDrawAction(card));
            if (CanFreelyDraw(card)) actions.Add(GetFreelyDrawAction(card));
            if (CanBury(card)) actions.Add(GetBuryAction(card));
            return actions;
        }

        bool CanDisplay(CardInstance card)
        {
            // Can't display if already displayed.
            if (card.Owner?.DisplayedCards.Contains(card) == true)
                return false;

            // Can't display if another armor was played on the check.
            if (Check?.StagedCardTypes.Contains(card.Data.cardType) == true)
                return false;

            // If we're in an encounter...
            if (_contexts.EncounterContext != null)
            {
                // We can only display if there's a DamageResolvable for this card's owner.
                var resolvable = _contexts.CurrentResolvable as DamageResolvable;
                if (resolvable != null && resolvable.PlayerCharacter == card.Owner)
                    return true;

                return false;
            }

            // If we made it here, no reason we can't play it.
            return true;
        }

        bool CanDraw(CardInstance card) => (
            // We can draw for damage if displayed and we have a DamageResolvable for the card's owner with Combat damage.
            Check != null
            && card.Owner.DisplayedCards.Contains(card)
            && !Check.StagedCardTypes.Contains(PF.CardType.Armor)
            && _contexts.CurrentResolvable is DamageResolvable resolvable
            && resolvable.DamageType == "Combat"
            && resolvable.PlayerCharacter == card.Owner);

        bool CanFreelyDraw(CardInstance card) => (
            // We can draw for damage if displayed and we have a DamageResolvable for the card's owner with Combat damage.
            Check != null
            && card.Owner.DisplayedCards.Contains(card)
            && _asm.CardStaged(card) // If we staged the Display this check, we can freely draw.
            && _contexts.CurrentResolvable is DamageResolvable resolvable
            && resolvable.DamageType == "Combat"
            && resolvable.PlayerCharacter == card.Owner);

        bool CanBury(CardInstance card) => (
            // We can bury for damage if displayed, the owner is proficient, and we have a DamageResolvable for the card's owner.
            Check != null
            && card.Owner.DisplayedCards.Contains(card)
            && (_asm.CardStaged(card) || !Check.StagedCardTypes.Contains(PF.CardType.Armor)) // If we staged the Display this check, we can freely bury.
            && card.Owner.IsProficient(PF.CardType.Armor)
            && _contexts.CurrentResolvable is DamageResolvable resolvable
            && resolvable.PlayerCharacter == card.Owner);
    }
}
