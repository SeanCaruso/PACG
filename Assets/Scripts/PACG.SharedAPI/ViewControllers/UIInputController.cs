using PACG.Data;
using PACG.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Header("Action Staging Flow")]
        public Button cancelButton;
        public Button commitButton;
        public Button skipButton;

        [Header("Character Input")]
        public GameObject power1Button;
        public GameObject power2Button;
        public GameObject power3Button;

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
            GameEvents.PlayerPowerEnabled += PlayerPowerEnabled;
            GameEvents.PlayerCharacterChanged += UpdatePlayerArea;
            GameEvents.StagedActionsStateChanged += UpdateStagedActionButtons;
            GameEvents.TurnStateChanged += UpdateTurnButtons;
        }

        protected void Start()
        {
            // Hook up button clicks.
            //giveCardButton.onClick.AddListener(() => GiveCardButton_OnClick());
            //moveButton.onClick.AddListener(() => MoveButton_OnClick());
            exploreButton.onClick.AddListener(ExploreButton_OnClick);
            optionalDiscardButton.onClick.AddListener(() => _gameFlowManager.StartPhase(new EndTurnController(false, _gameServices), "Turn"));
            endTurnButton.onClick.AddListener(() => _gameFlowManager.StartPhase(new EndTurnController(true, _gameServices), "Turn"));

            cancelButton.onClick.AddListener(() => _asm.Cancel());
            commitButton.onClick.AddListener(() => _asm.Commit());
            skipButton.onClick.AddListener(() => _asm.Commit());
        }

        protected void OnDestroy()
        {
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

        private void UpdatePlayerArea(PlayerCharacter pc)
        {
            _powerButtonMap.Clear();
            foreach (var button in PowerButtons)
            {
                button.GetComponent<Image>().enabled = false;
                button.GetComponent<Button>().enabled = false;
            }

            var activatedPowers = pc.CharacterData.powers.Where(power => power.isActivated);
            int idx = 0;
            foreach (var power in activatedPowers)
            {
                if (idx > 2) break;

                var image = PowerButtons[idx].GetComponent<Image>();
                image.sprite = power.spriteDisabled;
                image.enabled = true;

                PowerButtons[idx].GetComponent<Button>().enabled = false;

                _powerButtonMap.Add(power, PowerButtons[idx]);

                ++idx;
            }
        }

        private void PlayerPowerEnabled(CharacterPower power, bool isEnabled, IResolvable powerResolvable)
        {
            if (!_powerButtonMap.TryGetValue(power, out var buttonObj))
            {
                Debug.LogError($"[{GetType().Name}] Couldn't find CharacterPower in the map!");
                return;
            }

            buttonObj.GetComponent<Image>().sprite = isEnabled ? power.spriteEnabled : power.spriteDisabled;

            var button = buttonObj.GetComponent<Button>();
            button.enabled = isEnabled;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                if (_contexts.CurrentResolvable is PlayerPowerAvailableResolvable res)
                    res.DoNextResolvable();
                _asm.Commit();
            });
        }

        private void UpdateTurnButtons()
        {
            giveCardButton.enabled = false;
            moveButton.enabled = false;
            exploreButton.enabled = false;
            optionalDiscardButton.enabled = false;
            endTurnButton.enabled = false;

            // If we're mid-resolvable, everything should be disabled.
            if (_contexts.CurrentResolvable != null) return;

            var turn = _contexts.TurnContext;
            giveCardButton.enabled = turn.CanGive;
            moveButton.enabled = turn.CanMove;
            exploreButton.enabled = turn.CanFreelyExplore;
            optionalDiscardButton.enabled = turn.Character.Hand.Count > 0;
            endTurnButton.enabled = true;
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

            exploreButton.enabled = state.IsExploreEnabled;
        }
    }
}

