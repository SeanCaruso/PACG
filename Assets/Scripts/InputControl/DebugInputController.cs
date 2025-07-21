using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugInputController : MonoBehaviour, IInputController
{
    private List<PlayCardAction> currentOptions;
    private PlayCardAction selectedAction;
    private bool waitingForInput;

    public PlayCardAction SelectedAction => selectedAction;

    void Update()
    {
        if (!waitingForInput) return;

        for (int i = 0; i < currentOptions.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                selectedAction = currentOptions[i];
                break;
            }
        }
    }

    public IEnumerator PresentCardActionChoices(List<PlayCardAction> actionChoices, ActionContext context)
    {
        currentOptions = actionChoices;
        selectedAction = null;
        waitingForInput = true;

        Debug.Log("--- Choose a card to play: ---");
        for (int i = 0; i < actionChoices.Count; i++)
        {
            Debug.Log($"{i + 1}) {actionChoices[i].GetLabel()}");
        }

        yield return new WaitUntil(() => selectedAction != null);

        waitingForInput = false;
        yield return selectedAction;
    }

    public void CommitActions()
    {
        throw new System.NotImplementedException();
    }

    public void UndoAction()
    {
        throw new System.NotImplementedException();
    }
}
