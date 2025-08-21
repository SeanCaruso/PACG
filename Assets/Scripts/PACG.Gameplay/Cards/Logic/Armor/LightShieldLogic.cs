using System.Collections.Generic;

namespace PACG.Gameplay
{
    public class LightShieldLogic : CardLogicBase
    {
        private readonly ActionStagingManager _asm;
        private readonly ContextManager _contexts;

        private CheckContext Check => _contexts.CheckContext;

        public LightShieldLogic(GameServices gameServices) : base(gameServices) 
        {
            _asm = gameServices.ASM;
            _contexts = gameServices.Contexts;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            if (CanReveal(card))
                return new List<IStagedAction>
                {
                    new PlayCardAction(card, PF.ActionType.Reveal, ("IsFreely", true), ("Damage", 1))
                };
            if (CanRecharge(card))
                return new List<IStagedAction>
                {
                    new PlayCardAction(card, PF.ActionType.Recharge, ("IsFreely", true))
                };
            
            return new List<IStagedAction>();
        }

        // Can freely reveal once if the owner has a Combat DamageResolvable.
        private bool CanReveal(CardInstance card) =>
            !_asm.CardStaged(card)
            && _contexts.CurrentResolvable is DamageResolvable { DamageType: "Combat" } resolvable
            && resolvable.PlayerCharacter == card.Owner;

        private bool CanRecharge(CardInstance card) => 
            // We can freely recharge to reroll if we're processing a RerollResolvable and the dice pool has a d4, d6, or d8.
            Check != null
            && _contexts.CurrentResolvable is RerollResolvable resolvable
            && card.Owner == resolvable.Character
            && Check.UsedSkill == PF.Skill.Melee
            && resolvable.DicePool.NumDice(4, 6, 8) > 0;

        public override void OnStage(CardInstance card, IStagedAction action)
        {
            _contexts.EncounterContext.AddProhibitedTraits(card.Owner, card, "2-Handed");
        }

        public override void OnUndo(CardInstance card, IStagedAction action)
        {
            _contexts.EncounterContext.UndoProhibitedTraits(card.Owner, card);
        }
    }
}
