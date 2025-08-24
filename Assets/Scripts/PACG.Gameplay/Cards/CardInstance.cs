using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace PACG.Gameplay
{
    public class CardInstance : ICard
    {
        // Interface properties
        public string Name => Data.cardName;
        public List<string> Traits => Data.traits;
        
        // Passed in via constructor
        public CardData Data { get; }
        public CardLogicBase Logic { get; }
        public PlayerCharacter OriginalOwner { get; set; }
        public PlayerCharacter Owner { get; set; }

        // GUID that never changes
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Guid InstanceId { get; }

        // Initialized to a default value, updated during gameplay
        public CardLocation CurrentLocation { get; set; }

        public override string ToString() => Data.cardName;

        public CardInstance(CardData data, CardLogicBase logic, PlayerCharacter owner = null)
        {
            Data = Object.Instantiate(data);
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

        public List<IStagedAction> GetAvailableActions() => Logic?.GetAvailableActions(this) ?? new List<IStagedAction>();

        public IResolvable GetOnEncounterResolvable() => Logic?.GetOnEncounterResolvable(this);
        public IResolvable GetBeforeActingResolvable() => Logic?.GetBeforeActingResolvable(this);
        public IResolvable GetCheckResolvable() => Logic?.GetCheckResolvable(this);
    }

    public enum CardLocation { Buried, Deck, Discard, Displayed, Hand, Recovery, Revealed, Vault }
}
