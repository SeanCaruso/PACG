using PACG.Data;
using PACG.Gameplay;
using PACG.Presentation;
using UnityEngine;

namespace PACG.SharedAPI
{
    /// <summary>
    /// Controller for turn/encounter-specific cards.
    /// </summary>
    public class TurnCardDisplayController : MonoBehaviour
    {
        [Header("Dependencies")]
        public CardDisplayFactory CardDisplayFactory;
        public LocationDisplayFactory LocationDisplayFactory;
        
        [Header("Card Areas")]
        public RectTransform LocationContainer;
        public RectTransform HoursContainer;
        public RectTransform EncounteredContainer;

        private LocationDisplay _currentLocationDisplay;
        
        protected void Awake()
        {
            GameEvents.PcLocationChanged += OnPcLocationChanged;
            GameEvents.HourChanged += OnHourChanged;
            GameEvents.EncounterStarted += OnEncounterStarted;
            GameEvents.EncounterEnded += OnEncounterEnded;
            
            GameEvents.LocationPowerEnabled += OnLocationPowerEnabled;
        }

        protected void OnDestroy()
        {
            GameEvents.PcLocationChanged -= OnPcLocationChanged;
            GameEvents.HourChanged -= OnHourChanged;
            GameEvents.EncounterStarted -= OnEncounterStarted;
            GameEvents.EncounterEnded -= OnEncounterEnded;
            
            GameEvents.LocationPowerEnabled -= OnLocationPowerEnabled;
        }

        // ========================================================================================
        // TURN AND ENCOUNTER MANAGEMENT
        // ========================================================================================

        private void OnPcLocationChanged(PlayerCharacter pc, Location location)
        {
            if (_currentLocationDisplay)
                Destroy(_currentLocationDisplay.gameObject);
            
            _currentLocationDisplay = LocationDisplayFactory.CreateLocationDisplay(
                location,
                LocationDisplayFactory.DisplayContext.Default,
                LocationContainer
            );
        }

        private void OnHourChanged(CardInstance hourCard)
        {
            if (HoursContainer.childCount  == 1)
                Destroy(HoursContainer.GetChild(0).gameObject);
            
            CardDisplayFactory.CreateCardDisplay(
                hourCard,
                CardDisplayFactory.DisplayContext.Default,
                HoursContainer
            );
        }

        private void OnEncounterStarted(CardInstance encounteredCard)
        {
            CardDisplayFactory.CreateCardDisplay(
                encounteredCard,
                CardDisplayFactory.DisplayContext.Default,
                EncounteredContainer);
        }

        private void OnEncounterEnded()
        {
            if (EncounteredContainer.childCount == 1)
                Destroy(EncounteredContainer.GetChild(0).gameObject);
        }

        private void OnLocationPowerEnabled(LocationPower power, bool isEnabled)
        {
            switch (power.PowerType)
            {
                case LocationPowerType.AtLocation:
                    _currentLocationDisplay.SetAtLocationPowerEnabled(isEnabled);
                    if (isEnabled)
                        _currentLocationDisplay.AtLocationButton.onClick.AddListener(() => power.OnActivate?.Invoke());
                    else
                        _currentLocationDisplay.AtLocationButton.onClick.RemoveAllListeners();
                    break;
                case LocationPowerType.ToClose:
                    _currentLocationDisplay.SetToClosePowerEnabled(isEnabled);
                    if (isEnabled)
                        _currentLocationDisplay.ToCloseButton.onClick.AddListener(() => power.OnActivate?.Invoke());
                    else
                        _currentLocationDisplay.ToCloseButton.onClick.RemoveAllListeners();
                    break;
                case LocationPowerType.WhenClosed:
                    _currentLocationDisplay.SetWhenClosedPowerEnabled(isEnabled);
                    if (isEnabled)
                        _currentLocationDisplay.WhenClosedButton.onClick.AddListener(() => power.OnActivate?.Invoke());
                    else
                        _currentLocationDisplay.WhenClosedButton.onClick.RemoveAllListeners();
                    break;
            }
        }
    }
}
