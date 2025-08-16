using PACG.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PACG.SharedAPI
{
    public class PlayerChoiceController : MonoBehaviour
    {
        [Header("UI Elements")]
        public Transform ActionButtonContainer;
        public GameObject ButtonPrefab;
        
        // Dependency injection
        private ContextManager _contexts;
        private GameFlowManager _gameFlow;

        private void Awake()
        {
            GameEvents.PlayerChoiceEvent += OnPlayerChoiceEvent;
        }

        private void OnDestroy()
        {
            GameEvents.PlayerChoiceEvent -= OnPlayerChoiceEvent;
        }

        public void Initialize(GameServices gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
        }

        private void OnPlayerChoiceEvent(PlayerChoiceResolvable resolvable)
        {
            GameEvents.SetStatusText(resolvable.Prompt);

            foreach (var option in resolvable.Options)
            {
                var buttonObj = Instantiate(ButtonPrefab, ActionButtonContainer);
                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = option.Label;

                buttonObj.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    EndChoice();
                    _contexts.EndResolvable();
                    GameEvents.RaiseTurnStateChanged();
                    option.Action?.Invoke();
                    _gameFlow.Process();
                });
            }
        }

        private void EndChoice()
        {
            GameEvents.SetStatusText("");

            for (var i = 0; i < ActionButtonContainer.childCount; i++)
            {
                Destroy(ActionButtonContainer.GetChild(i).gameObject);
            }
        }
    }
}
