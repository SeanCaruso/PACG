using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionManager
{
    public LogicRegistry LogicRegistry;
    public ResolutionManager(LogicRegistry LogicRegistry)
    {
        this.LogicRegistry = LogicRegistry;
    }

    public IEnumerator HandleCombatResolvable(CombatResolvable combat, ActionContext context, IInputController input)
    {
        var actions = combat.GetValidActions(context);
        yield return input.PresentCardActionChoices(actions, context);

        input.SelectedAction.Commit(context);
    }
}
