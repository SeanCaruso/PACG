using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    public class ResolveEncounterProcessor : BaseProcessor
    {
        private readonly CardManager _cardManager;
        private readonly ContextManager _contexts;

        public ResolveEncounterProcessor(GameServices gameServices)
            : base(gameServices)
        {
            _cardManager = gameServices.Cards;
            _contexts = gameServices.Contexts;
        }

        protected override void OnExecute()
        {
            var encounteredCard = _contexts.EncounterContext.Card;
            bool wasSuccess = _contexts.EncounterContext.CheckResult.WasSuccess;

            bool banish = (encounteredCard.IsBane && wasSuccess) || (encounteredCard.IsBoon && !wasSuccess);

            if (banish)
            {
                _cardManager.MoveCard(encounteredCard, CardLocation.Vault);
            }
            else if (encounteredCard.IsBane)
            {
                // Shuffle undefeated banes back into the location deck.
                // TODO: Get ResolveEncounter resolvables (e.g. option to banish if undefeated, etc.)
                // TODO: Handle summoned cards.
                _contexts.EncounterPcLocation.ShuffleIn(encounteredCard);
            }
            else
            {
                // Player acquires defeated boon.
                _contexts.EncounterContext.Character.AddToHand(encounteredCard);
            }

            GameEvents.RaiseEncounterEnded();

            _contexts.EndEncounter();
        }
    }
}
