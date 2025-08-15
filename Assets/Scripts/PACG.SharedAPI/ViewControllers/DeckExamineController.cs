using PACG.Gameplay;
using PACG.Presentation;
using System.Collections.Generic;
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

        // Other members
        private ExamineResolvable _resolvable;
        private readonly Dictionary<GameObject, CardInstance> cardDisplayObjectsToInstances = new();

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
            _resolvable = resolvable;

            // Clears any old states.
            PrepareExamine();

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
                cardDisplay.gameObject.AddComponent<CardDragHandler>().Initialize(this);

                cardDisplayObjectsToInstances.Add(cardDisplay.gameObject, card);
            }

            GameObject buttonObj = Instantiate(ButtonPrefab, ButtonContainer);
            buttonObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = "Continue";
            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                CleanUpExamine();
                _asm.Commit();
                ExamineArea.SetActive(false);
            });
        }

        private void PrepareExamine()
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

        private void CleanUpExamine()
        {
            foreach (var cardDisplay in cardDisplayObjectsToInstances.Keys)
            {
                Destroy(cardDisplay.GetComponent<CardDragHandler>());
            }
            cardDisplayObjectsToInstances.Clear();
        }

        public void SwapCards(GameObject cardA, GameObject cardB)
        {
            if (_resolvable == null) return;

            if (!_resolvable.CanReorder) return;

            // Swap visual positions.
            int indexA = cardA.transform.GetSiblingIndex();
            int indexB = cardB.transform.GetSiblingIndex();

            cardA.transform.SetSiblingIndex(indexB);
            cardB.transform.SetSiblingIndex(indexA);

            // Swap instances in the CurrentOrder list.
            var instanceA = cardDisplayObjectsToInstances[cardA];
            var instanceB = cardDisplayObjectsToInstances[cardB];

            var currentOrder = _resolvable.CurrentOrder;
            int listIndexA = currentOrder.IndexOf(instanceA);
            int listIndexB = currentOrder.IndexOf(instanceB);

            if (listIndexA < 0 || listIndexB < 0)
                return;

            Debug.Log($"[{GetType().Name}] Swapping {instanceA} with {instanceB}");

            (currentOrder[listIndexA], currentOrder[listIndexB]) = (currentOrder[listIndexB], currentOrder[listIndexA]);
        }
    }
}
