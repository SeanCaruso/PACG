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
