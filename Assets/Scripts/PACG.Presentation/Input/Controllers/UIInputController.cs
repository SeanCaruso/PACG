using System;
using UnityEngine;
using UnityEngine.UI;

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

    public static UIInputController Instance { get; private set; }

    private void Awake()
    {
        // Standard singleton setup
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    // --- Turn Flow -----------------------------------------

    // TODO: OnGiveCardButtonClicked
    // TODO: OnMoveButtonClicked

    public event Action OnExploreButtonClicked;
    public void ExploreButton_OnClick() => OnExploreButtonClicked?.Invoke();

    public event Action OnOptionalDiscardButtonClicked;
    public void OptionalDiscardButton_OnClick() => OnOptionalDiscardButtonClicked?.Invoke();

    public event Action OnEndTurnButtonClicked;
    public void EndTurnButton_OnClick() => OnEndTurnButtonClicked?.Invoke();

    // --- Action Staging Flow -----------------------------------

    public event Action OnCancelButtonClicked;
    public void CancelButton_OnClick() => OnCancelButtonClicked?.Invoke();

    public event Action OnCommitButtonClicked;
    public void CommitButton_OnClick() => OnCommitButtonClicked?.Invoke();

    public event Action OnSkipButtonClicked;
    public void SkipButton_OnClick() => OnSkipButtonClicked?.Invoke();
}
 
