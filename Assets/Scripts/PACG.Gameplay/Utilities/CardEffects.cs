
namespace PACG.Gameplay
{
    public static class CardEffects
    {
        public static PlayerChoiceResolvable CreateExploreChoice(GameServices gameServices)
        {
            var gameFlow = gameServices.GameFlow;
            
            return new PlayerChoiceResolvable(
                "Explore?",
                new PlayerChoiceResolvable.ChoiceOption(
                    "Explore",
                    () => gameFlow.QueueNextProcessor(new Turn_ExploreProcessor(gameServices))),
                new PlayerChoiceResolvable.ChoiceOption(
                    "Forfeit\nExploration",
                    () => { })
            );
        }
    }
}
