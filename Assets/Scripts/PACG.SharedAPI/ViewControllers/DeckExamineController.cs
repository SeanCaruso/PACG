using PACG.Gameplay;
using PACG.Presentation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PACG.SharedAPI
{
    public class DeckExamineController : MonoBehaviour
    {
        [Header("Other Controllers")]
        public CardDisplayController CardDisplayController;

        [Header("UI Elements")]
        public GameObject ExamineArea;
        public Transform CardBacksContainer;
        public Transform CardsContainer;
        public Transform ButtonContainer;

        [Header("Prefabs")]
        public GameObject ButtonPrefab;
        public CardDisplay CardPrefab;
        public GameObject CardBackPrefab;

        // Dependency injections
        private ActionStagingManager _asm;

        public void Awake()
        {
            DialogEvents.ExamineEvent += OnExamineEvent;
        }

        public void OnDestroy()
        {
            DialogEvents.ExamineEvent -= OnExamineEvent;
        }

        public void Initialize(GameServices gameServices)
        {
            _asm = gameServices.ASM;
        }

        private void OnExamineEvent(ExamineResolvable resolvable)
        {
            ClearExamine();

            ExamineArea.SetActive(true);

            int numCardBacks = resolvable.DeckSize - resolvable.Count;
            for (int i = 0; i  < numCardBacks; i++)
            {
                Instantiate(CardBackPrefab, CardBacksContainer);
            }

            foreach (var card in resolvable.ExaminedCards)
            {
                CardDisplay cardDisplay = CardDisplayController.GetCardDisplay(card);
                cardDisplay.transform.SetParent(CardsContainer, worldPositionStays: false);
            }

            GameObject buttonObj = Instantiate(ButtonPrefab, ButtonContainer);
            buttonObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = "Continue";
            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                _asm.Commit();
                ExamineArea.SetActive(false);
            });
        }

        private void ClearExamine()
        {
            for (int i = 0; i < CardBacksContainer.childCount; i++)
            {
                Destroy(CardBacksContainer.GetChild(i).gameObject);
            }

            for (int i = 0; i < CardsContainer.childCount; i++)
            {
                CardDisplayController.HideCard(CardsContainer.GetChild(i).GetComponent<CardDisplay>());
            }

            if (ButtonContainer.childCount == 1)
                Destroy(ButtonContainer.GetChild(0).gameObject);
        }
    }
}
