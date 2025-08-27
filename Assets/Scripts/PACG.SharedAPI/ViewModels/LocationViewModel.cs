using System.Collections.Generic;
using System.Linq;
using PACG.Core;
using PACG.Data;
using PACG.Gameplay;
using UnityEngine;

namespace PACG.SharedAPI
{
    public class LocationViewModel
    {
        public LocationViewModel(Location location)
        {
            Location = location;
            _data = location.LocationData;
        }

        // The underlying CardInstance
        public Location Location { get; }
        private LocationData _data;

        // Ready-to-display strings
        public string Name => Location.Name.ToUpper();
        public string Level => $"{_data.Level}";
        
        // At this Location
        public string AtLocationText => StringUtils.ReplaceAdventureLevel(
            _data.AtLocationPower.Text, CardUtils.AdventureNumber
        );
        public bool HasAtLocationPower => _data.AtLocationPower.IsActivated;
        public Sprite AtLocationEnabledSprite => _data.AtLocationPower.SpriteEnabled;
        public Sprite AtLocationDisabledSprite => _data.AtLocationPower.SpriteDisabled;
        
        // To Close or To Guard
        public string ToCloseText => StringUtils.ReplaceAdventureLevel(
            _data.ToClosePower.Text, CardUtils.AdventureNumber
        );
        public bool HasToClosePower => _data.ToClosePower.IsActivated;
        public Sprite ToCloseEnabledSprite => _data.ToClosePower.SpriteEnabled;
        public Sprite ToCloseDisabledSprite => _data.ToClosePower.SpriteDisabled;
        
        // When Closed
        public string WhenClosedText => StringUtils.ReplaceAdventureLevel(
            _data.WhenClosedPower.Text, CardUtils.AdventureNumber
        );
        public bool HasWhenClosedPower => _data.WhenClosedPower.IsActivated;
        public Sprite WhenClosedEnabledSprite => _data.WhenClosedPower.SpriteEnabled;
        public Sprite WhenClosedDisabledSprite => _data.WhenClosedPower.SpriteDisabled;

        // Ready-to-use values
        public IEnumerable<string> Traits => _data.Traits.Select(trait => trait.ToUpper());
    }
}
