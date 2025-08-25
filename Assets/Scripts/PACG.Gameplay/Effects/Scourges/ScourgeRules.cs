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
