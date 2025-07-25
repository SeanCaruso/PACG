using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionManager
{
    public LogicRegistry LogicRegistry;
    private UIInputController uiController;

    public ResolutionManager(LogicRegistry LogicRegistry, UIInputController uiController)
    {
        this.LogicRegistry = LogicRegistry;
        this.uiController = uiController;
    }

    public IEnumerator HandleCombatResolvable(CombatResolvable combat, ActionContext context)
    {
        var actions = combat.GetValidActions(context);
        yield return uiController.PresentCardActionChoices(actions, context);

        if (uiController.SelectedAction != null)
        {
            // Stage the action (for any pre-execution effects)
            uiController.SelectedAction.playable?.OnStage(context, uiController.SelectedAction.PowerIndex);

            // Commit the action.
            uiController.SelectedAction.Commit(context);
        }
    }
}
