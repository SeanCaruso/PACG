using System.Collections.Generic;
using System.Linq;
using PACG.Core;

namespace PACG.Gameplay
{
    public class VoidglassArmorLogic : CardLogicBase, IOnBeforeDiscardResponse
    {
        // Dependency injections
        private readonly ActionStagingManager _asm;
        private readonly CardManager _cardManager;
        private readonly ContextManager _contexts;

        public VoidglassArmorLogic(GameServices gameServices) : base(gameServices)
        {
            _asm = gameServices.ASM;
            _cardManager = gameServices.Cards;
            _contexts = gameServices.Contexts;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (CanDisplay(card))
                actions.Add(new PlayCardAction(card, ActionType.Display, null));
            if (CanRechargeForDamage(card))
                actions.Add(new PlayCardAction(card, ActionType.Recharge, null, ("Damage", 1)));
            if (CanFreelyRechargeForDamage(card))
                actions.Add(new PlayCardAction(card, ActionType.Recharge, null, ("Damage", 1), ("IsFreely", true)));
            if (CanBury(card))
                actions.Add(new PlayCardAction(card, ActionType.Bury, null, ("ReduceDamageTo", 0)));
            if (CanFreelyBury(card))
                actions.Add(new PlayCardAction(card, ActionType.Bury, null, ("ReduceDamageTo", 0), ("IsFreely", true)));
            return actions;
        }

        private bool CanDisplay(CardInstance card)
        {
            // Can't display if already displayed.
            if (card.Owner?.DisplayedCards.Contains(card) == true)
                return false;

            // Can't display if another armor was played on the check.
            if (_contexts.CurrentResolvable?.IsCardTypeStaged(card.CardType) == true)
                return false;

            // If there's no encounter or resolvable...
            if (_contexts.EncounterContext == null && _contexts.CurrentResolvable == null)
                return true; // ... we can display.

            // Otherwise, We can only display if there's a DamageResolvable for this card's owner.
            if (_contexts.CurrentResolvable is DamageResolvable resolvable && resolvable.PlayerCharacter == card.Owner)
                return true;

            return false;
        }

        // We can recharge for damage if displayed and we have a DamageResolvable for the card's owner.
        private bool CanRechargeForDamage(CardInstance card) =>
            card.Owner.DisplayedCards.Contains(card)
            && _contexts.CurrentResolvable is DamageResolvable resolvable
            && !resolvable.IsCardTypeStaged(card.CardType)
            && resolvable.PlayerCharacter == card.Owner;

        // We can also freely recharge if the card was displayed for this damage resolution.
        private bool CanFreelyRechargeForDamage(CardInstance card) =>
            card.Owner.DisplayedCards.Contains(card)
            && _asm.CardStaged(card)
            && _contexts.CurrentResolvable is DamageResolvable resolvable
            && resolvable.PlayerCharacter == card.Owner;

        // We can bury for damage if displayed, the owner is proficient, and we have a DamageResolvable for the card's owner.
        private bool CanBury(CardInstance card) =>
            card.Owner.DisplayedCards.Contains(card)
            && card.Owner.IsProficient(card.Data)
            && _contexts.CurrentResolvable is DamageResolvable resolvable
            && !resolvable.IsCardTypeStaged(card.CardType)
            && resolvable.PlayerCharacter == card.Owner;

        // We can also freely bury if the card was displayed for this damage resolution.
        private bool CanFreelyBury(CardInstance card) =>
            card.Owner.DisplayedCards.Contains(card)
            && _asm.CardStaged(card)
            && card.Owner.IsProficient(card.Data)
            && _contexts.CurrentResolvable is DamageResolvable resolvable
            && resolvable.PlayerCharacter == card.Owner;

        public void OnBeforeDiscard(CardInstance sourceCard, DiscardEventArgs args)
        {
            if (sourceCard.Owner != args.Character) return;

            // Only respond if displayed or in hand.
            if (sourceCard.CurrentLocation is not (CardLocation.Hand or CardLocation.Displayed)) return;

            // If we're not discarding from the deck or dealing mental damage, we can't use this card.
            if (!(args.OriginalLocation == CardLocation.Deck || args.DamageResolvable?.DamageType == "Mental")) return;

            var offer = new CardResponse(
                sourceCard,
                $"Recharge {sourceCard}",
                AcceptAction
            );
            args.CardResponses.Add(offer);
            return;

            void AcceptAction()
            {
                _cardManager.MoveCard(sourceCard, ActionType.Recharge);
                // If this is for a Mental DamageResolvable, override the default action to Recharge.
                if (args.DamageResolvable?.DamageType == "Mental")
                {
                    args.DamageResolvable?.OverrideActionType(ActionType.Recharge);
                    _asm.Commit();
                }

                // If this is for discarding cards from the deck, recharge them instead.
                foreach (var cardToRecharge in args.Cards)
                    _cardManager.MoveCard(cardToRecharge, ActionType.Recharge);
            }
        }
    }
}
