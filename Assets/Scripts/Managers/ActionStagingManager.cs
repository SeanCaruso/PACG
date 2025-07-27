using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ActionStagingManager : MonoBehaviour
{
    private readonly Stack<IStagedAction> stagedActions = new();
    private readonly Dictionary<CardData, CardStagingInfo> originalCardStates = new();

    public Stack<IStagedAction> StagedActions => stagedActions;

    [Header("UI References")]
    public GameObject commitButton;
    public GameObject undoButton;
    public Transform revealedArea;
    public Transform displayedArea;

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    public void StageAction(IStagedAction action, CardStagingInfo? overrideOrigin = null)
    {
        var cardDisplay = FindCardDisplay(action.CardData);
        if (cardDisplay == null)
        {
            Debug.LogError($"StageAction --- Unable to find cardDisplay for {action.CardData.cardName}");
        }

        // Keep track of where the card will be going if undone.
        if (overrideOrigin.HasValue)
        {
            originalCardStates[action.CardData] = overrideOrigin.Value;
        }
        else
        {
            originalCardStates[action.CardData] = new()
            {
                cardDisplay = cardDisplay,
                originalScale = cardDisplay.transform.localScale,
                originalParent = cardDisplay.transform.parent,
                originalSiblingIndex = cardDisplay.transform.GetSiblingIndex()
            };
        }

        // Hide/move the card based on the action type.
        switch (action.ActionType)
        {
            case PF.ActionType.Display:
                // Move to display area.
                cardDisplay.transform.SetParent(displayedArea);
                cardDisplay.transform.localScale = new(.6f, .6f);
                break;
            case PF.ActionType.Reveal:
                // Move to reveal area.
                cardDisplay.transform.SetParent(revealedArea);
                cardDisplay.transform.localScale = new(.6f, .6f);
                break;
            default:
                // Hide the card.
                cardDisplay.gameObject.SetActive(false);
                break;
        }

        stagedActions.Push(action);
        UpdateUI();
    }

    public void Undo()
    {
        var action = stagedActions.Pop();

        if (!originalCardStates.TryGetValue(action.CardData, out var stagingInfo))
        {
            Debug.LogError($"Unable to find original card state for {action.CardData.cardName}");
            return;
        }

        stagingInfo.cardDisplay.transform.SetParent(stagingInfo.originalParent);
        stagingInfo.cardDisplay.transform.SetSiblingIndex(stagingInfo.originalSiblingIndex);
        stagingInfo.cardDisplay.transform.localScale = stagingInfo.originalScale;
        stagingInfo.cardDisplay.gameObject.SetActive(true);

        UpdateUI();
    }

    private void UpdateUI()
    {
        undoButton.SetActive(stagedActions.Count > 0);
        commitButton.SetActive(Game.ResolutionContext.IsResolved(stagedActions));
    }

    public void Commit()
    {
        foreach (var action in stagedActions)
        {
            action.Commit();
        }
        stagedActions.Clear();
        Game.ResolutionContext?.Resolve();

        // Move all revealed cards back to the hand.
        foreach (Transform cardTranform in revealedArea)
        {
            var cardDisplay = cardTranform.GetComponent<CardDisplay>();
            if (cardDisplay != null && originalCardStates.TryGetValue(cardDisplay.CardData, out var stagingInfo))
            {
                cardTranform.SetParent(stagingInfo.originalParent);
                cardTranform.localScale = stagingInfo.originalScale;
                cardTranform.SetSiblingIndex(stagingInfo.originalSiblingIndex);
            }
        }

        undoButton.SetActive(false);
        commitButton.SetActive(false);
    }

    private CardDisplay FindCardDisplay(CardData cardData)
    {
        return FindObjectsByType<CardDisplay>(FindObjectsSortMode.None).FirstOrDefault(cd => cd.CardData == cardData);
    }
}

public struct CardStagingInfo
{
    public CardDisplay cardDisplay;
    public Transform originalParent;
    public Vector3 originalScale;
    public int originalSiblingIndex;
}