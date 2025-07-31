using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ActionStagingManager : MonoBehaviour
{
    private readonly Dictionary<PlayerCharacter, List<IStagedAction>> pcsStagedActions = new();
    private readonly Dictionary<CardInstance, CardStagingInfo> originalCardStates = new();

    [Header("UI References")]
    public GameObject commitButton;
    public Sprite commitSprite;
    public Sprite skipSprite;
    public GameObject cancelButton;
    public Transform handArea;
    public Transform revealedArea;
    public Transform displayedArea;

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    public void StageAction(IStagedAction action, CardStagingInfo? overrideOrigin = null)
    {
        var cardDisplay = FindCardDisplay(action.Card);
        if (cardDisplay == null)
        {
            Debug.LogError($"StageAction --- Unable to find cardDisplay for {action.Card.Data.cardName}. Action not staged!");
            return;
        }

        // If this is the first time we've staged an action for this card, keep track of where the card will be going if undone.
        if (!originalCardStates.ContainsKey(action.Card))
        {
            if (overrideOrigin.HasValue)
            {
                originalCardStates[action.Card] = overrideOrigin.Value;
            }
            else
            {
                originalCardStates[action.Card] = new()
                {
                    cardDisplay = cardDisplay,
                    originalParent = cardDisplay.transform.parent,
                    originalCharacterLocation = action.Card.CurrentLocation,
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
            case PF.ActionType.Draw:
                cardDisplay.transform.SetParent(handArea);
                cardDisplay.transform.localScale = new(1f, 1f);
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
        action.Card.Owner.MoveCard(action.Card, action.ActionType);

        action.OnStage();

        var pcActions = pcsStagedActions.GetValueOrDefault(action.Card.Owner, new());
        pcActions.Add(action);
        pcsStagedActions[action.Card.Owner] = pcActions;
        UpdateUI();
    }

    public void Cancel()
    {
        // TODO: Get currently displayed PC. Use current turn PC for now.
        PlayerCharacter pc = Game.TurnContext.CurrentPC;
        foreach (var action in pcsStagedActions[pc])
        {
            if (!originalCardStates.TryGetValue(action.Card, out var stagingInfo))
            {
                Debug.LogError($"Unable to find original card state for {action.Card.Data.cardName}");
                return;
            }

            stagingInfo.cardDisplay.transform.SetParent(stagingInfo.originalParent);
            stagingInfo.cardDisplay.transform.SetSiblingIndex(stagingInfo.originalSiblingIndex);
            stagingInfo.cardDisplay.transform.localScale = stagingInfo.originalScale;
            stagingInfo.cardDisplay.gameObject.SetActive(true);

            // Restore the card back to the correct location in the data.
            Game.MoveCard(action.Card, stagingInfo.originalCharacterLocation);
            //if (action.ActionType != PF.ActionType.Reveal)
            //{
            //    stagingInfo.originalCharacterLocation.Add(action.CardData);
            //}

            action.OnUndo();
        }
        pcsStagedActions[pc].Clear();
        originalCardStates.Clear();

        UpdateUI();
    }

    public void UpdateUI()
    {
        // TODO: Update this for the displayed PC. Use Turn PC until then.
        var pc = Game.TurnContext.CurrentPC;
        var stagedActions = pcsStagedActions.GetValueOrDefault(pc) ?? (pcsStagedActions[pc] = new());
        cancelButton.SetActive(stagedActions.Count > 0);

        bool canCommit = stagedActions.Count > 0 && (Game.ResolutionContext?.IsResolved(stagedActions) ?? true); // We have actions but no resolvable? We can commit!
        bool canSkip = stagedActions.Count == 0 && (Game.ResolutionContext?.IsResolved(new()) ?? false); // We don't have any actions and no resolvable to skip, so false!
        commitButton.SetActive(canCommit || canSkip);
        commitButton.GetComponent<Image>().sprite = canSkip ? skipSprite : commitSprite;
    }

    public void Commit()
    {
        foreach (var action in pcsStagedActions.Values.SelectMany(list => list))
        {
            action.Commit();
        }
        pcsStagedActions.Clear();
        Game.ResolutionContext?.Resolve();

        // Move all revealed cards back to the hand.
        // TODO: This might break when we have multiple PCs.
        while (revealedArea.childCount > 0)
        {
            var cardTransform = revealedArea.GetChild(0);
            var cardDisplay = cardTransform.GetComponent<CardDisplay>();
            if (cardDisplay != null && originalCardStates.TryGetValue(cardDisplay.Card, out var stagingInfo))
            {
                cardTransform.SetParent(stagingInfo.originalParent);
                cardTransform.localScale = stagingInfo.originalScale;
                cardTransform.SetSiblingIndex(stagingInfo.originalSiblingIndex);
            }
        }
        originalCardStates.Clear();

        cancelButton.SetActive(false);
        commitButton.SetActive(false);
    }

    private CardDisplay FindCardDisplay(CardInstance card)
    {
        return FindObjectsByType<CardDisplay>(FindObjectsSortMode.None).FirstOrDefault(cd => cd.Card == card);
    }
}

public struct CardStagingInfo
{
    public CardDisplay cardDisplay;
    public Transform originalParent;
    public CardLocation originalCharacterLocation;
    public Vector3 originalScale;
    public int originalSiblingIndex;
}