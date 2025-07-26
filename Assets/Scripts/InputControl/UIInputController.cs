using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInputController : MonoBehaviour, IInputController
{
    [Header("UI References")]
    public Transform actionButtonContainer;
    public GameObject actionButtonPrefab;
    public Button commitButton;
    public Button undoButton;
    public TextMeshProUGUI statusText;
    public GameObject diceRollPanel;
    public TextMeshProUGUI diceRollText;

    private List<PlayCardAction> currentOptions;
    private PlayCardAction selectedAction;
    //private bool waitingForInput;
    private List<GameObject> activeButtons = new();

    public PlayCardAction SelectedAction => selectedAction;

    void Start()
    {
        if (commitButton) commitButton.onClick.AddListener(CommitActions);
        if (undoButton) undoButton.onClick.AddListener(UndoAction);

        SetUIState(false);
    }

    public IEnumerator PresentCardActionChoices(List<PlayCardAction> actionChoices, ActionContext context)
    {
        currentOptions = actionChoices;
        selectedAction = null;
        //waitingForInput = true;

        if (statusText) statusText.text = "Choose a card to play:";

        ClearActionButtons();
        CreateActionButtons();

        SetUIState(true);

        yield return new WaitUntil(() => selectedAction != null);

        SetUIState(false);
        //waitingForInput = false;
    }

    private void CreateActionButtons()
    {
        if (!actionButtonContainer || !actionButtonPrefab) return;

        for (int i = 0; i < currentOptions.Count; i++)
        {
            GameObject buttonObj = Instantiate(actionButtonPrefab, actionButtonContainer);
            activeButtons.Add(buttonObj);

            // Set up the button.
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText) buttonText.text = currentOptions[i].GetLabel();

            // Capture the action for the lambda.
            PlayCardAction action = currentOptions[i];
            button.onClick.AddListener(() => OnActionSelected(action));

            // Optional: Add card display component if you want to show card art.
            //CardActionButton cardButton = buttonObj.GetComponent<CardActionButton>();
            //if (cardButton) cardButton.Setup(currentOptions[i]);
        }
    }

    private void ClearActionButtons()
    {
        foreach (GameObject button in activeButtons)
        {
            if (button) Destroy(button);
        }
        activeButtons.Clear();
    }

    private void OnActionSelected(PlayCardAction action)
    {
        selectedAction = action;

        // Visual feedback
        if (statusText) statusText.text = $"Selected: {action.GetLabel()}";

        // Disable all buttons to prevent double-clicking
        foreach (GameObject buttonObj in activeButtons)
        {
            Button button = buttonObj.GetComponent<Button>();
            if (button) button.interactable = false;
        }
    }

    private void SetUIState(bool active)
    {
        if (actionButtonContainer) actionButtonContainer.gameObject.SetActive(active);
        if (commitButton) commitButton.gameObject.SetActive(active);
        if (undoButton) undoButton.gameObject.SetActive(active);
        if (statusText) statusText.gameObject.SetActive(active);
    }

    public void CommitActions()
    {
        // This could be extended for multiple actions.
        Debug.Log("Actions committed.");
    }

    public void UndoAction()
    {
        // Reset selection.
        selectedAction = null;

        // Re-enable buttons.
        foreach (GameObject buttonObj in activeButtons)
        {
            Button button = buttonObj.GetComponent<Button>();
            if (button) button.interactable = true;
        }

        if (statusText) statusText.text = "Choose a card to play:";
    }

    // Show dice roll results with animation.
    public IEnumerator ShowDiceRoll(DicePool dicePool, int result)
    {
        if (!diceRollPanel || !diceRollText) yield break;

        diceRollPanel.SetActive(true);

        // Show the dice being "rolled" with some animation.
        for (int i = 0; i < 10; i++)
        {
            diceRollText.text = $"Rolling {dicePool}...\n{dicePool.Roll()}";
            yield return new WaitForSeconds(0.1f);
        }

        // Show final result.
        diceRollText.text = $"Rolled {dicePool}...\n{dicePool.Roll()}";

        yield return new WaitForSeconds(2f);
        diceRollPanel.SetActive(false);
    }
}
 
