using System.Linq;

namespace PACG.Gameplay
{
    public enum ScourgeType
    {
        Dazed,
        Drained,
        Entangled,
        Exhausted,
        Frightened,
        Poisoned,
        Wounded
    }

    public static class ScourgeRules
    {
        // =============================================================================================================
        // EXHAUSTED
        // =============================================================================================================
        public static void PromptForExhaustedRemoval(PlayerCharacter pc, GameServices gameServices)
        {
            var resolvable = new PlayerChoiceResolvable("Remove Exhausted?",
                new PlayerChoiceResolvable.ChoiceOption("Yes", () =>
                {
                    pc.RemoveScourge(ScourgeType.Exhausted);
                    gameServices.GameFlow.StartPhase(
                        new EndTurnController(false, gameServices),
                        "End Turn"
                    );
                }),
                new PlayerChoiceResolvable.ChoiceOption("No", () => { })
            );

            var processor = new NewResolvableProcessor(resolvable, gameServices);
            gameServices.GameFlow.StartPhase(processor, "Exhausted Removal");
        }

        // =============================================================================================================
        // WOUNDED
        // =============================================================================================================
        public static void HandleWoundedDeckDiscard(PlayerCharacter pc, GameServices gameServices)
        {
            var topCard = new[] { pc.Deck.DrawCard() };
            var args = new DiscardEventArgs(pc, topCard.ToList(), CardLocation.Deck);
            gameServices.Cards.TriggerBeforeDiscard(args);

            if (args.HasResponses)
            {
                var options = args.CardResponses.Select(response =>
                    new PlayerChoiceResolvable.ChoiceOption(response.Description, response.OnAccept)
                ).ToList();
                options.Add(new PlayerChoiceResolvable.ChoiceOption("Skip", DefaultDiscard));

                var choices = new PlayerChoiceResolvable("Use Power?", options.ToArray());
                var processor = new NewResolvableProcessor(choices, gameServices);
                gameServices.GameFlow.StartPhase(processor, "Wound Discard Options");
            }
            else
                DefaultDiscard();

            return;

            void DefaultDiscard() => gameServices.Cards.MoveCard(topCard[0], CardLocation.Discard);
        }

        public static void PromptForWoundedRemoval(PlayerCharacter pc, GameServices gameServices)
        {
            var resolvable = new PlayerChoiceResolvable("Remove Wounded?",
                new PlayerChoiceResolvable.ChoiceOption("Yes", () => { pc.RemoveScourge(ScourgeType.Wounded); }),
                new PlayerChoiceResolvable.ChoiceOption("No", () => { })
            );

            var processor = new NewResolvableProcessor(resolvable, gameServices);
            gameServices.GameFlow.StartPhase(processor, "Wound Removal");
        }
    }
}
