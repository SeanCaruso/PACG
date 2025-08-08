
using System;
using UnityEngine;

namespace PACG.Gameplay
{
    public class CardInstance
    {
        // Passed in via constructor
        public CardData Data { get; private set; }
        public PlayerCharacter OriginalOwner { get; set; }
        public PlayerCharacter Owner { get; set; }

        // GUID that never changes
        public Guid InstanceId { get; private set; }

        // Initialized to a default value, updated during gameplay
        public CardLocation CurrentLocation { get; set; }

        public CardInstance(CardData data, PlayerCharacter owner = null)
        {
            Data = data;
            InstanceId = Guid.NewGuid();
            CurrentLocation = CardLocation.Vault;
            OriginalOwner = owner;
            Owner = owner;
        }
    }

    public enum CardLocation { Buried, Deck, Discard, Displayed, Hand, Recovery, Revealed, Vault }
}