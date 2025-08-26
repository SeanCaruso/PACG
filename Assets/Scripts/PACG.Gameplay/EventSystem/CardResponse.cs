using System;

namespace PACG.Gameplay
{
    public class CardResponse
    {
        public CardInstance RespondingCard { get; }
        public string Description { get; }
        public Action OnAccept { get; }

        public CardResponse(CardInstance respondingCard, string description, Action onAccept)
        {
            RespondingCard = respondingCard;
            Description = description;
            OnAccept = onAccept;
        }
    }
}
