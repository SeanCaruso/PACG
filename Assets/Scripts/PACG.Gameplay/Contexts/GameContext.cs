
namespace PACG.Gameplay
{
    public class GameContext
    {
        public int AdventureNumber { get; }
        public Deck HourDeck { get; }

        public GameContext(int adventureNumber, CardManager cardManager)
        {
            AdventureNumber = adventureNumber;
            HourDeck = new(cardManager);
        }
    }
}
