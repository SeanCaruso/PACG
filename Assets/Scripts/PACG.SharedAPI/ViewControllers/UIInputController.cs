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
            exploreButton.onClick.AddListener(() => _gameFlowManager.StartPhase(new Turn_ExploreProcessor(_gameServices), "Explore"));
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

        protected void UpdatePlayerArea(PlayerCharacter pc)
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

        protected void PlayerPowerEnabled(CharacterPower power, bool enabled, IResolvable powerResolvable)
        {
            if (!_powerButtonMap.ContainsKey(power))
            {
                Debug.LogError($"[{GetType().Name}] Couldn't find CharacterPower in the map!");
                return;
            }

            var buttonObj = _powerButtonMap[power];
            buttonObj.GetComponent<Image>().sprite = enabled ? power.spriteEnabled : power.spriteDisabled;

            var button = buttonObj.GetComponent<Button>();
            button.enabled = enabled;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                if (_contexts.CurrentResolvable is PlayerPowerAvailableResolvable res)
                    res.DoNextResolvable();
                _asm.Commit();
            });
        }

        protected void UpdateTurnButtons()
        {
            giveCardButton.enabled = false;
            moveButton.enabled = false;
            exploreButton.enabled = false;
            optionalDiscardButton.enabled = false;

            // If we're mid-resolvable, everything should be disabled.
            if (_contexts.CurrentResolvable != null) return;

            TurnContext turn = _contexts.TurnContext;
            giveCardButton.enabled = turn.CanGive;
            moveButton.enabled = turn.CanMove;
            exploreButton.enabled = turn.CanExplore;
            optionalDiscardButton.enabled = turn.Character.Hand.Count > 0;
        }

        // --- Action Staging Flow -----------------------------------

        protected void UpdateStagedActionButtons(StagedActionsState state)
        {
            cancelButton.gameObject.SetActive(state.IsCancelButtonVisible);
            commitButton.gameObject.SetActive(state.IsCommitButtonVisible);
            skipButton.gameObject.SetActive(state.IsSkipButtonVisible);
        }
    }
}

