

namespace PACG.Gameplay
{
    public class TurnContext
    {
        public CardInstance HourBlessing { get; }
        public PlayerCharacter CurrentPC { get; }
        public Deck LocationDeck { get; set; }

        public bool CanGive { get; set; }
        public bool CanMove { get; set; }
        public bool CanExplore { get; set; }
        public bool CanCloseLocation { get; set; }
        public bool CanEndTurn { get; set; }

        public TurnContext(CardInstance hourBlessing, PlayerCharacter currentPC, Deck locationDeck)
        {
            HourBlessing = hourBlessing;
            CurrentPC = currentPC;
            LocationDeck = locationDeck;
        }
    }
}
