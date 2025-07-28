using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPreviewController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject previewArea;
    public Button backgroundButton;

    [Header("Action Buttons")]
    public Transform actionButtonContainer;
    public GameObject actionButtonPrefab;

    private CardDisplay currentlyEnlargedCard;
    private Transform originalParent;
    private int originalSiblingIndex;
    private Vector3 originalScale;

    private readonly List<GameObject> activeActionButtons = new();

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    private void Start()
    {
        // Add a listener to the background button to handle returning the card.
        backgroundButton.onClick.AddListener(ReturnCardToOrigin);
        previewArea.SetActive(false);
    }

    public void EnlargeCard(GameObject cardToEnlarge)
    {
        if (currentlyEnlargedCard != null) return;

        CardDisplay cardDisplay = cardToEnlarge.GetComponent<CardDisplay>();
        if (cardDisplay == null )
        {
            Debug.LogError($"{GetType().Name}.EnlargeCard --- Unable to get card display component!");
            return;
        }

        currentlyEnlargedCard = cardDisplay;

        // Store the card's original location and size.
        originalParent = cardToEnlarge.transform.parent;
        originalSiblingIndex = cardToEnlarge.transform.GetSiblingIndex();
        originalScale = cardToEnlarge.transform.localScale;

        // Show the preview area.
        previewArea.SetActive(true);

        // Move the card to the preview area and enlarge it.
        var cardRect = cardToEnlarge.GetComponent<RectTransform>();
        cardRect.SetParent(previewArea.transform, false);
        cardRect.anchoredPosition = Vector3.zero;
        cardRect.anchorMin = new(0.5f, 0.5f);
        cardRect.anchorMax = new(0.5f, 0.5f);
        cardRect.localScale = new Vector3(2f, 2f, 1.0f);

        // Generate any action buttons for the current context.
        var actions = Game.ResolutionContext?.ValidActionsForCard(cardDisplay.CardData) ?? new();
        if (actions.Count > 0)
        {
            GenerateActionButtons(actions);
        }
    }

    public void GenerateActionButtons(List<IStagedAction> actions)
    {
        CardStagingInfo stagingInfo = new()
        {
            cardDisplay = currentlyEnlargedCard,
            originalParent = originalParent,
            originalScale = Vector3.one,
            originalSiblingIndex = originalSiblingIndex
        };

        foreach (var action in actions)
        {
            stagingInfo.originalCharacterLocation = Game.TurnContext.CurrentPC.FindCard(action.CardData);

            GameObject buttonObj = Instantiate(actionButtonPrefab, actionButtonContainer);

            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = action.ActionType.ToString();
            Button button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                ServiceLocator.Get<ActionStagingManager>().StageAction(action, stagingInfo);
                EndPreview();
            });

            activeActionButtons.Add(buttonObj);
        }
    }

    private void ReturnCardToOrigin()
    {
        if (currentlyEnlargedCard == null) return;

        // Return the card to its original parent and Z-index.
        currentlyEnlargedCard.transform.SetParent(originalParent, false);
        currentlyEnlargedCard.transform.SetSiblingIndex(originalSiblingIndex);
        currentlyEnlargedCard.transform.localScale = originalScale;

        // Clear action buttons.
        EndPreview();
    }

    private void EndPreview()
    {
        // Hide the preview and clear the card.
        previewArea.SetActive(false);
        currentlyEnlargedCard = null;

        // Remove any action buttons.
        foreach (var button in activeActionButtons)
        {
            Destroy(button);
        }
        activeActionButtons.Clear();
    }
}
