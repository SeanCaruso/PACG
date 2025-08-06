
namespace PACG.Gameplay
{
    public class GameContext
    {
        public int AdventureNumber { get; }
        public Deck HourDeck { get; } = new();

        public GameContext(int adventureNumber)
        {
            AdventureNumber = adventureNumber;
        }
    }
}
