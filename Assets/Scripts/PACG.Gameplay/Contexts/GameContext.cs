
namespace PACG.Gameplay
{
    public class GameContext
    {
        public int AdventureNumber { get; private set; }

        public GameContext(int adventureNumber)
        {
            AdventureNumber = adventureNumber;
        }
    }
}
