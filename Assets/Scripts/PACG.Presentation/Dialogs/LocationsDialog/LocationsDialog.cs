using PACG.Gameplay;
using PACG.SharedAPI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PACG.Presentation.LocationsDialog
{
    public class LocationsDialog : MonoBehaviour
    {
        [Header("UI Elements")]
        public Transform LocationButtonsContainer;
        public Transform LocationPreviewContainer;
        public GameObject CancelButton;
        public GameObject ConfirmButton;

        [Header("Images")]
        public Sprite ConfirmEnabled;
        public Sprite ConfirmDisabled;
        
        [Header("Prefabs")]
        public GameObject ActionButtonPrefab;

        private ActionStagingManager _asm;
        private ContextManager _contexts;
        
        private PlayerCharacter _pc;
        private LocationDisplayFactory _locationDisplayFactory;

        private void OnEnable()
        {
            CancelButton.GetComponent<Button>().onClick.AddListener(() => Destroy(gameObject));
        }
        
        private void OnDisable()
        {
            CancelButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }

        public void Initialize(LocationDisplayFactory locationDisplayFactory, GameServices gameServices, PlayerCharacter pc)
        {
            _asm = gameServices.ASM;
            _contexts = gameServices.Contexts;
            _pc = pc;
            _locationDisplayFactory = locationDisplayFactory;
            
            foreach (var location in _contexts.GameContext.Locations)
            {
                var buttonObj = Instantiate(ActionButtonPrefab, LocationButtonsContainer);
                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = location.Name;

                if (!buttonObj.TryGetComponent<Button>(out var button)) continue;
                button.onClick.AddListener(() => DisplayPreview(location));
            }
            
            DisplayPreview(pc.Location);
        }

        private void DisplayPreview(Location location)
        {
            if (LocationPreviewContainer.childCount > 0)
                Destroy(LocationPreviewContainer.GetChild(0).gameObject);
            
            _locationDisplayFactory.CreateLocationDisplay(
                location,
                LocationDisplayFactory.DisplayContext.GameStateIndicator,
                LocationPreviewContainer);
            
            var confirmButton = ConfirmButton.GetComponent<Button>();
            confirmButton.enabled = location != _pc.Location;
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() =>
            {
                _contexts.GameContext.MoveCharacter(_pc, location);
                _contexts.TurnContext.CanGive = false;
                _contexts.TurnContext.CanMove = false;
                GameEvents.RaiseTurnStateChanged();
                Destroy(gameObject);
                _asm.Commit();
            });
            
            var confirmImage = confirmButton.GetComponent<Image>();
            confirmImage.sprite = confirmButton.enabled ? ConfirmEnabled : ConfirmDisabled;
        }
    }
}
