using PACG.Gameplay;
using PACG.Presentation;
using UnityEngine;

namespace PACG.SharedAPI
{
    public class ScenarioViewController : MonoBehaviour
    {
        [Header("UI Elements")]
        public Transform ScenarioPowerContainer;
        
        [Header("Prefabs")]
        public ScenarioPower ScenarioPowerPrefab;
        
        private ScenarioPower _scenarioPower;
        
        // Dependency injection
        private ContextManager _contexts;

        private void OnEnable()
        {
            GameEvents.ScenarioHasPower += OnScenarioPower;
            GameEvents.ScenarioPowerEnabled += OnScenarioPowerEnabled;
            GameEvents.TurnStateChanged += UpdateStagedActionButtons;
        }
        
        private void OnDisable()
        {
            GameEvents.ScenarioHasPower -= OnScenarioPower;
            GameEvents.ScenarioPowerEnabled -= OnScenarioPowerEnabled;
            GameEvents.TurnStateChanged -= UpdateStagedActionButtons;
        }

        private void OnScenarioPower(GameServices gameServices)
        {
            _contexts = gameServices.Contexts;
            
            _scenarioPower = Instantiate(ScenarioPowerPrefab, ScenarioPowerContainer);

            _scenarioPower.PowerText.text = _contexts.GameContext.ScenarioData.DuringScenario;
            _scenarioPower.PowerButton.enabled = false;
            _scenarioPower.PowerButton.onClick.AddListener(_contexts.GameContext.ScenarioLogic.InvokeAction);
            _scenarioPower.PowerIcon.sprite = _contexts.GameContext.ScenarioData.DuringScenarioPowerDisabled;
            
            _scenarioPower.PowerIcon.gameObject.SetActive(_contexts.GameContext.ScenarioData.IsDuringPowerActivated);
        }

        private void OnScenarioPowerEnabled(bool isEnabled)
        {
            if (!_scenarioPower) return;
            
            _scenarioPower.PowerButton.enabled = isEnabled;
            
            _scenarioPower.PowerIcon.sprite = isEnabled 
                ? _contexts.GameContext.ScenarioData.DuringScenarioPowerEnabled 
                : _contexts.GameContext.ScenarioData.DuringScenarioPowerDisabled;
        }

        private void UpdateStagedActionButtons()
        {
            if (!_scenarioPower) return;
            
            OnScenarioPowerEnabled(_contexts.GameContext.ScenarioLogic.HasAvailableAction);
        }
    }
}
