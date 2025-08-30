using PACG.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PACG.SharedAPI
{
    public class PlayerChoiceController : MonoBehaviour
    {
        [Header("Dependencies")]
        public CardDisplayFactory CardDisplayFactory;
        public LocationDisplayFactory LocationDisplayFactory;

        [Header("UI Elements")]
        public Transform ActionButtonContainer;
        public Transform CardPreviewContainer;

        [Header("Prefabs")]
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

            RectTransform cardRect;
            Transform traitsSection;
            switch (resolvable.Card)
            {
                case CardInstance cardInstance:
                {
                    var cardDisplay = CardDisplayFactory.CreateCardDisplay(
                        cardInstance,
                        CardDisplayFactory.DisplayContext.Preview,
                        CardPreviewContainer);

                    cardRect = cardDisplay.gameObject.GetComponent<RectTransform>();
                    traitsSection = cardDisplay.transform.Find("Traits_Area"); // or however it's named
                    break;
                }
                case Location location:
                {
                    var locationDisplay = LocationDisplayFactory.CreateLocationDisplay(
                        location,
                        LocationDisplayFactory.DisplayContext.Preview,
                        CardPreviewContainer);

                    cardRect = locationDisplay.gameObject.GetComponent<RectTransform>();
                    traitsSection = locationDisplay.transform.Find("Traits_Area"); // or however it's named
                    break;
                }
                default:
                    return;
            }
            
            cardRect.anchoredPosition = Vector3.zero;
            cardRect.anchorMin = new Vector2(0.5f, 0.5f);
            cardRect.anchorMax = new Vector2(0.5f, 0.5f);
            if (traitsSection)
                LayoutRebuilder.ForceRebuildLayoutImmediate(traitsSection.GetComponent<RectTransform>());

            CardPreviewContainer.parent.gameObject.SetActive(true);
        }

        private void EndChoice()
        {
            GameEvents.SetStatusText("");

            for (var i = 0; i < ActionButtonContainer.childCount; i++)
            {
                Destroy(ActionButtonContainer.GetChild(i).gameObject);
            }

            for (var i = 0; i < CardPreviewContainer.childCount; i++)
            {
                Destroy(CardPreviewContainer.GetChild(i).gameObject);
            }
            CardPreviewContainer.parent.gameObject.SetActive(false);
        }
    }
}
