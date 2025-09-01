using System.Collections.Generic;
using System.Linq;
using PACG.Data;
using PACG.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace PACG.SharedAPI
{
    public class UIInputController : MonoBehaviour
    {
        [Header("Turn Flow")]
        public Button giveCardButton;
        public Button moveButton;
        public Button exploreButton; // The location deck.
        public Button optionalDiscardButton;
        public Button endTurnButton;

        [Header("Turn Flow Sprites")]
        public Sprite GiveCardEnabled;
        public Sprite GiveCardDisabled;
        public Sprite MoveEnabled;
        public Sprite MoveDisabled;
        public Sprite ExploreEnabled;
        public Sprite ExploreDisabled;
        public Sprite OptionalDiscardEnabled;
        public Sprite OptionalDiscardDisabled;
        public Sprite EndTurnEnabled;
        public Sprite EndTurnDisabled;

        [Header("Action Staging Flow")]
        public Button cancelButton;
        public Button commitButton;
        public Button skipButton;

        [Header("Character Input")]
        public GameObject power1Button;
        public GameObject power2Button;
        public GameObject power3Button;

        private PlayerCharacter _selectedPc;
        private List<GameObject> PowerButtons => new() { power1Button, power2Button, power3Button };
        private readonly Dictionary<CharacterPower, GameObject> _powerButtonMap = new();

        // Dependency injections set in Initialize
        private ActionStagingManager _asm;
        private ContextManager _contexts;
        private GameFlowManager _gameFlowManager;
        private GameServices _gameServices;

        public void Awake()
        {
            // Subscribe to events.
            GameEvents.PlayerCharacterChanged += OnPlayerCharacterChanged;
            GameEvents.PlayerPowerEnabled += PlayerPowerEnabled;
            GameEvents.PlayerCharacterChanged += UpdatePlayerArea;
            GameEvents.StagedActionsStateChanged += UpdateStagedActionButtons;
            GameEvents.TurnStateChanged += UpdateTurnButtons;
        }

        protected void Start()
        {
            // Hook up button clicks.
            //giveCardButton.onClick.AddListener(() => GiveCardButton_OnClick());
            moveButton.onClick.AddListener(() =>
                DialogEvents.RaiseMoveClickedEvent(_contexts.TurnContext.Character, _gameServices));
            exploreButton.onClick.AddListener(ExploreButton_OnClick);
            optionalDiscardButton.onClick.AddListener(() =>
                _gameFlowManager.StartPhase(new EndTurnController(false, _gameServices), "Turn"));
            endTurnButton.onClick.AddListener(() =>
                _gameFlowManager.StartPhase(new EndTurnController(true, _gameServices), "Turn"));

            cancelButton.onClick.AddListener(() => _asm.Cancel());
            commitButton.onClick.AddListener(() => _asm.Commit());
            skipButton.onClick.AddListener(() => _asm.Skip());
        }

        protected void OnDestroy()
        {
            GameEvents.PlayerCharacterChanged -= OnPlayerCharacterChanged;
            GameEvents.PlayerPowerEnabled -= PlayerPowerEnabled;
            GameEvents.PlayerCharacterChanged -= UpdatePlayerArea;
            GameEvents.StagedActionsStateChanged -= UpdateStagedActionButtons;
            GameEvents.TurnStateChanged -= UpdateTurnButtons;
        }

        public void Initialize(GameServices gameServices)
        {
            _asm = gameServices.ASM;
            _contexts = gameServices.Contexts;
            _gameFlowManager = gameServices.GameFlow;
            _gameServices = gameServices;
        }

        // --- Turn Flow -----------------------------------------
        private void OnPlayerCharacterChanged(PlayerCharacter pc)
        {
            _selectedPc = pc;
            UpdatePlayerArea(pc);
            UpdateTurnButtons();
        }

        private void UpdatePlayerArea(PlayerCharacter pc)
        {
            _powerButtonMap.Clear();
            foreach (var button in PowerButtons)
            {
                button.GetComponent<Image>().enabled = false;
                button.GetComponent<Button>().enabled = false;
            }

            var activatedPowers = pc.CharacterData.Powers.Where(power => power.IsActivated);
            var idx = 0;
            foreach (var power in activatedPowers)
            {
                if (idx > 2) break;

                var image = PowerButtons[idx].GetComponent<Image>();
                image.sprite = power.SpriteDisabled;
                image.enabled = true;

                PowerButtons[idx].GetComponent<Button>().enabled = false;

                _powerButtonMap.Add(power, PowerButtons[idx]);

                ++idx;
            }
        }

        private void PlayerPowerEnabled(CharacterPower power, bool isEnabled)
        {
            if (!_powerButtonMap.TryGetValue(power, out var buttonObj))
            {
                Debug.LogError($"[{GetType().Name}] Couldn't find CharacterPower in the map!");
                return;
            }

            buttonObj.GetComponent<Image>().sprite = isEnabled ? power.SpriteEnabled : power.SpriteDisabled;

            var button = buttonObj.GetComponent<Button>();
            button.enabled = isEnabled;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => power.OnActivate?.Invoke());
        }

        private void UpdateTurnButtons()
        {
            if (_contexts.CurrentResolvable != null 
                || _contexts.TurnContext == null
                || _selectedPc != _contexts.TurnContext?.Character)
            {
                UpdateButton(giveCardButton, false, GiveCardDisabled);
                UpdateButton(moveButton, false, MoveDisabled);
                UpdateButton(exploreButton, false, ExploreDisabled);
                UpdateButton(optionalDiscardButton, false, OptionalDiscardDisabled);
                UpdateButton(endTurnButton, false, EndTurnDisabled);
            }
            else
            {
                var turn = _contexts.TurnContext;
                UpdateButton(giveCardButton, turn.CanGive, turn.CanGive ? GiveCardEnabled : GiveCardDisabled);
                UpdateButton(moveButton, turn.CanMove, turn.CanMove ? MoveEnabled : MoveDisabled);
                UpdateButton(exploreButton, turn.CanFreelyExplore,
                    turn.CanFreelyExplore ? ExploreEnabled : ExploreDisabled);
                UpdateButton(optionalDiscardButton, turn.Character.Hand.Count > 0,
                    turn.Character.Hand.Count > 0 ? OptionalDiscardEnabled : OptionalDiscardDisabled);
                UpdateButton(endTurnButton, true, EndTurnEnabled);
            }
        }

        private static void UpdateButton(Button button, bool isEnabled, Sprite sprite)
        {
            button.enabled = isEnabled;
            button.gameObject.GetComponent<Image>().sprite = sprite;
        }

        private void ExploreButton_OnClick()
        {
            _asm.Commit();
            _gameFlowManager.StartPhase(new Turn_ExploreProcessor(_gameServices), "Explore");
        }

        // --- Action Staging Flow -----------------------------------

        private void UpdateStagedActionButtons(StagedActionsState state)
        {
            cancelButton.gameObject.SetActive(state.IsCancelButtonVisible);
            commitButton.gameObject.SetActive(state.IsCommitButtonVisible);
            skipButton.gameObject.SetActive(state.IsSkipButtonVisible);

            UpdateButton(moveButton, state.IsMoveEnabled, state.IsMoveEnabled ? MoveEnabled : MoveDisabled);
            
            UpdateButton(exploreButton, state.IsExploreEnabled,
                state.IsExploreEnabled ? ExploreEnabled : ExploreDisabled);
        }
    }
}
