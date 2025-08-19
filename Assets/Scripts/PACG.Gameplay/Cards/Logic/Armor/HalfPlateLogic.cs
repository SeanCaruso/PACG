using System.Collections.Generic;
using System.Linq;

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
                actions.Add(new PlayCardAction(card, PF.ActionType.Display));
            if (CanDraw(card))
                actions.Add(new PlayCardAction(card, PF.ActionType.Draw, ("Damage", 2)));
            if (CanFreelyDraw(card))
                actions.Add(new PlayCardAction(card, PF.ActionType.Draw, ("Damage", 2), ("IsFreely", true)));
            if (CanBury(card))
                actions.Add(new PlayCardAction(card, PF.ActionType.Bury, ("ReduceDamageTo", 0)));
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

        private bool CanDraw(CardInstance card) => 
            // We can draw for damage if displayed and we have a DamageResolvable for the card's owner with Combat damage.
            Check != null
            && card.Owner.DisplayedCards.Contains(card)
            && !Check.StagedCardTypes.Contains(PF.CardType.Armor)
            && _contexts.CurrentResolvable is DamageResolvable { DamageType: "Combat" } resolvable
            && resolvable.PlayerCharacter == card.Owner;

        private bool CanFreelyDraw(CardInstance card) =>
            // We can draw for damage if displayed and we have a DamageResolvable for the card's owner with Combat damage.
            Check != null
            && card.Owner.DisplayedCards.Contains(card)
            && _asm.CardStaged(card) // If we staged the Display this check, we can freely draw.
            && _contexts.CurrentResolvable is DamageResolvable { DamageType: "Combat" } resolvable
            && resolvable.PlayerCharacter == card.Owner;

        private bool CanBury(CardInstance card) =>
            // We can bury for damage if displayed, the owner is proficient, and we have a DamageResolvable for the card's owner.
            Check != null
            && card.Owner.DisplayedCards.Contains(card)
            && (_asm.CardStaged(card) || !Check.StagedCardTypes.Contains(card.Data.cardType)) // If we staged the Display this check, we can freely bury.
            && card.Owner.IsProficient(card.Data)
            && _contexts.CurrentResolvable is DamageResolvable resolvable
            && resolvable.PlayerCharacter == card.Owner;
    }
}
