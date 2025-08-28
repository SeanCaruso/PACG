using PACG.Gameplay;
using PACG.Presentation.LocationsDialog;
using UnityEngine;

namespace PACG.SharedAPI
{
    public class LocationsDialogViewController : MonoBehaviour
    {
        [Header("UI Elements")]
        public Transform LocationsDialogContainer;
        
        [Header("Prefabs")]
        public LocationsDialog LocationsDialogPrefab;

        [Header("Other Dependencies")]
        public LocationDisplayFactory LocationDisplayFactory;

        private void OnEnable()
        {
            DialogEvents.MoveClickedEvent += OnMoveClicked;
        }
        
        private void OnDisable()
        {
            DialogEvents.MoveClickedEvent -= OnMoveClicked;
        }

        private void OnMoveClicked(PlayerCharacter pc, GameServices gameServices)
        {
            var gui = Instantiate(LocationsDialogPrefab, LocationsDialogContainer);
            gui.Initialize(LocationDisplayFactory, gameServices, pc);
        }
    }
}
