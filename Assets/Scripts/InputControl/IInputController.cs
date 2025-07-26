using System.Collections;
using System.Collections.Generic;

public interface IInputController
{
    PlayCardAction SelectedAction { get; }

    IEnumerator PresentCardActionChoices(List<PlayCardAction> actionChoices);
    public void CommitActions();
    public void UndoAction();
}
