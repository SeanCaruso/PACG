using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static PF;

public class ActionStagingManager : MonoBehaviour
{
    private readonly Stack<IPlayableLogic> stagingOrder = new();
    private readonly Dictionary<IPlayableLogic, IStagedAction> stagedActions = new();
    private readonly Dictionary<CardData, CardStagingInfo> originalCardStates = new();

    //public Stack<IStagedAction> StagedActions => stagedActions;

    [Header("UI References")]
    public GameObject commitButton;
    public Sprite commitSprite;
    public Sprite skipSprite;
    public GameObject undoButton;
    public Transform revealedArea;
    public Transform displayedArea;

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    public void StageAction(IStagedAction action, CardStagingInfo? overrideOrigin = null)
    {
        var logic = Game.GetPlayableLogic(action.CardData);

        var cardDisplay = FindCardDisplay(action.CardData);
        if (cardDisplay == null)
        {
            Debug.LogError($"StageAction --- Unable to find cardDisplay for {action.CardData.cardName}");
        }

        // If this is the first time we've staged an action for this card, keep track of where the card will be going if undone.
        if (!stagedActions.ContainsKey(logic))
        {
            if (overrideOrigin.HasValue)
            {
                originalCardStates[action.CardData] = overrideOrigin.Value;
            }
            else
            {
                originalCardStates[action.CardData] = new()
                {
                    cardDisplay = cardDisplay,
                    originalParent = cardDisplay.transform.parent,
                    originalCharacterLocation = Game.TurnContext.CurrentPC.FindCard(action.CardData),
                    originalScale = cardDisplay.transform.localScale,
                    originalSiblingIndex = cardDisplay.transform.GetSiblingIndex()
                };
            }
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

        // We need to handle this here so that damage resolvables behave with hand size.
        Game.TurnContext.CurrentPC.MoveCard(action.CardData, action.ActionType);

        action.OnStage();

        stagedActions[logic] = (action);
        if (!stagingOrder.Contains(logic)) stagingOrder.Push(logic);
        UpdateUI();
    }

    public void Undo()
    {
        var logic = stagingOrder.Pop();
        if (!stagedActions.TryGetValue(logic, out var action))
        {
            Debug.LogError($"Unable to find staged action for {logic.CardData.cardName}!");
            return;
        }

        if (!originalCardStates.TryGetValue(action.CardData, out var stagingInfo))
        {
            Debug.LogError($"Unable to find original card state for {action.CardData.cardName}");
            return;
        }

        stagingInfo.cardDisplay.transform.SetParent(stagingInfo.originalParent);
        stagingInfo.cardDisplay.transform.SetSiblingIndex(stagingInfo.originalSiblingIndex);
        stagingInfo.cardDisplay.transform.localScale = stagingInfo.originalScale;
        stagingInfo.cardDisplay.gameObject.SetActive(true);

        // Restore the card back to the correct location in the data.
        if (action.ActionType != ActionType.Reveal)
        {
            stagingInfo.originalCharacterLocation.Add(action.CardData);
        }

        stagedActions.Remove(logic);
        action.OnUndo();

        UpdateUI();
    }

    public void UpdateUI()
    {
        undoButton.SetActive(stagedActions.Count > 0);

        bool canCommit = stagedActions.Count > 0 && Game.ResolutionContext.IsResolved(new(stagedActions.Values));
        bool canSkip = stagedActions.Count == 0 && Game.ResolutionContext.IsResolved(new());
        commitButton.SetActive(canCommit || canSkip);
        commitButton.GetComponent<Image>().sprite = canSkip ? skipSprite : commitSprite;
    }

    public void Commit()
    {
        foreach ((_, var action) in stagedActions)
        {
            action.Commit();
        }
        stagingOrder.Clear();
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
    public List<CardData> originalCharacterLocation;
    public Vector3 originalScale;
    public int originalSiblingIndex;
}