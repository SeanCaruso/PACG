using PACG.Gameplay;
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
        
        protected void Awake()
        {
            GameEvents.LocationChanged += OnLocationChanged;
            GameEvents.HourChanged += OnHourChanged;
            GameEvents.EncounterStarted += OnEncounterStarted;
            GameEvents.EncounterEnded += OnEncounterEnded;
        }

        protected void OnDestroy()
        {
            GameEvents.LocationChanged -= OnLocationChanged;
            GameEvents.HourChanged -= OnHourChanged;
            GameEvents.EncounterStarted -= OnEncounterStarted;
            GameEvents.EncounterEnded -= OnEncounterEnded;
        }

        // ========================================================================================
        // TURN AND ENCOUNTER MANAGEMENT
        // ========================================================================================

        private void OnLocationChanged(Location location)
        {
            if (LocationContainer.childCount == 1)
                Destroy(LocationContainer.GetChild(0).gameObject);
            
            LocationDisplayFactory.CreateLocationDisplay(
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
    }
}
