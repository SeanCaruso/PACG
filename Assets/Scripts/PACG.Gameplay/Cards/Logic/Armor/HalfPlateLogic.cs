using System.Collections.Generic;
using System.Linq;
using PACG.Core;

namespace PACG.Gameplay
{
    public class HalfPlateLogic : CardLogicBase
    {
        private readonly ContextManager _contexts;
        private readonly ActionStagingManager _asm;

        private CheckContext Check => _contexts.CheckContext;

        public HalfPlateLogic(GameServices gameServices) : base(gameServices) 
        {
            _contexts = gameServices.Contexts;
            _asm = gameServices.ASM;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (CanDisplay(card))
                actions.Add(new PlayCardAction(card, ActionType.Display));
            if (CanDraw(card))
                actions.Add(new PlayCardAction(card, ActionType.Draw, ("Damage", 2)));
            if (CanFreelyDraw(card))
                actions.Add(new PlayCardAction(card, ActionType.Draw, ("Damage", 2), ("IsFreely", true)));
            if (CanBury(card))
                actions.Add(new PlayCardAction(card, ActionType.Bury, ("ReduceDamageTo", 0)));
            if (CanFreelyBury(card))
                actions.Add(new PlayCardAction(card, ActionType.Bury, ("ReduceDamageTo", 0), ("IsFreely", true)));
            return actions;
        }

        private bool CanDisplay(CardInstance card)
        {
            // Can't display if already displayed.
            if (card.Owner?.DisplayedCards.Contains(card) == true)
                return false;

            // Can't display if another armor was played on the check.
            if (Check?.StagedCardTypes.Contains(card.Data.cardType) == true)
                return false;

            // If there's no encounter or resolvable...
            if (_contexts.EncounterContext == null && _contexts.CurrentResolvable == null)
                return true; // ... we can display.
            
            // Otherwise, We can only display if there's a DamageResolvable for this card's owner.
            if (_contexts.CurrentResolvable is DamageResolvable resolvable && resolvable.PlayerCharacter == card.Owner)
                return true;

            return false;
        }

        // We can draw for damage if displayed and we have a DamageResolvable for the card's owner with Combat damage.
        private bool CanDraw(CardInstance card) => 
            card.Owner.DisplayedCards.Contains(card)
            && _asm.StagedCards.Count(c => c.Data.cardType == card.Data.cardType) == 0
            && _contexts.CurrentResolvable is DamageResolvable { DamageType: "Combat" } resolvable
            && resolvable.PlayerCharacter == card.Owner;

        // We can also freely draw if the card was displayed for this damage resolution.
        private bool CanFreelyDraw(CardInstance card) =>
            card.Owner.DisplayedCards.Contains(card)
            && _asm.CardStaged(card)
            && _contexts.CurrentResolvable is DamageResolvable { DamageType: "Combat" } resolvable
            && resolvable.PlayerCharacter == card.Owner;

        // We can bury for damage if displayed, the owner is proficient, and we have a DamageResolvable for the card's owner.
        private bool CanBury(CardInstance card) =>
            card.Owner.DisplayedCards.Contains(card)
            && _asm.StagedCards.Count(c => c.Data.cardType == card.Data.cardType) == 0
            && card.Owner.IsProficient(card.Data)
            && _contexts.CurrentResolvable is DamageResolvable resolvable
            && resolvable.PlayerCharacter == card.Owner;

        // We can also freely bury if the card was displayed for this damage resolution.
        private bool CanFreelyBury(CardInstance card) =>
            card.Owner.DisplayedCards.Contains(card)
            && _asm.CardStaged(card)
            && card.Owner.IsProficient(card.Data)
            && _contexts.CurrentResolvable is DamageResolvable resolvable
            && resolvable.PlayerCharacter == card.Owner;
    }
}
