using System;
using System.Collections.Generic;

namespace PACG.Gameplay
{
    public class DiscardEventArgs : EventArgs
    {
        public PlayerCharacter Character { get; }
        public List<CardInstance> Cards { get; }
        public CardLocation OriginalLocation { get; }
        public DamageResolvable DamageResolvable { get; }
        
        public List<CardResponse> CardResponses { get; } = new();
        
        public bool HasResponses => CardResponses.Count > 0;

        public DiscardEventArgs(PlayerCharacter character,
            List<CardInstance> cards,
            CardLocation originalLocation,
            DamageResolvable resolvable = null)
        {
            Character = character;
            Cards = cards;
            OriginalLocation = originalLocation;
            DamageResolvable = resolvable;
        }
    }
}
