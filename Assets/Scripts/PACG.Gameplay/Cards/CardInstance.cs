
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class CardInstance
    {
        // Passed in via constructor
        public CardData Data { get; }
        public CardLogicBase Logic { get; }
        public PlayerCharacter OriginalOwner { get; set; }
        public PlayerCharacter Owner { get; set; }

        // GUID that never changes
        public Guid InstanceId { get; }

        // Initialized to a default value, updated during gameplay
        public CardLocation CurrentLocation { get; set; }

        public CardInstance(CardData data, CardLogicBase logic, PlayerCharacter owner = null)
        {
            Data = data;
            Logic = logic;
            InstanceId = Guid.NewGuid();
            CurrentLocation = CardLocation.Vault;
            OriginalOwner = owner;
            Owner = owner;
        }

        // ========================================================================================
        // CONVENIENCE PROPERTIES AND FUNCTIONS FOR COMMON CARD QUERIES
        // ========================================================================================

        public bool IsBane => PF.IsBane(Data.cardType);
        public bool IsBoon => PF.IsBoon(Data.cardType);

        // ========================================================================================
        // FACADE PATTERN - CONVENIENCE CALLS TO CARD LOGIC
        // ========================================================================================

        public List<IStagedAction> GetAvailableActions() => Logic?.GetAvailableActions(this) ?? new();

        public virtual List<IResolvable> GetOnEncounterResolvables() => Logic?.GetOnEncounterResolvables(this) ?? new();
        public virtual List<IResolvable> GetBeforeActingResolvables() => Logic?.GetBeforeActingResolvables(this) ?? new();
        public virtual List<IResolvable> GetCheckResolvables() => Logic?.GetCheckResolvables(this) ?? new();
    }

    public enum CardLocation { Buried, Deck, Discard, Displayed, Hand, Recovery, Revealed, Vault }
}