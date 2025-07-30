using UnityEngine;

public static class Game
{
    private static ContextManager _contextManager;

    public static void Initialize(ContextManager contextManager)
    {
        _contextManager = contextManager;
    }

    // --- Contexts ---
    public static GameContext GameContext => _contextManager.GameContext;

    public static TurnContext TurnContext => _contextManager.TurnContext;
    public static void NewTurn(TurnContext turnContext) => _contextManager.NewTurn(turnContext);
    public static void EndTurn() => _contextManager.EndTurn();

    public static EncounterContext EncounterContext => _contextManager.EncounterContext;
    public static void NewEncounter(EncounterContext encounterContext) => _contextManager.NewEncounter(encounterContext);
    public static void EndEncounter() => _contextManager.EndEncounter();

    public static ResolutionContext ResolutionContext => _contextManager.ResolutionContext;
    public static void NewResolution(ResolutionContext resolutionContext) => _contextManager.NewResolution(resolutionContext);
    public static void EndResolution() => _contextManager.EndResolution();

    public static CheckContext CheckContext => _contextManager.CheckContext;
    public static void NewCheck(CheckContext checkContext) => _contextManager.NewCheck(checkContext);
    public static void EndCheck() => _contextManager.EndCheck();
    public static void Stage(IStagedAction action)
    {
        // If we're not in a check, we don't care.
        if (CheckContext == null) return;

        // Skip if we've already staged the card.
        if (!CheckContext.StagedActions.Contains(action))
        {
            CheckContext.StagedActions.Add(action);
            if (!action.IsFreely)
            {
                if (CheckContext.StagedCardTypes.Contains(action.CardData.cardType)) Debug.Log($"{action.CardData.cardName} staged a duplicate type - was this intended?");
                CheckContext.StagedCardTypes.Add(action.CardData.cardType);
            }
        }
        else
        {
            Debug.Log($"{action.CardData.cardName}.{action} staged multiple times - was this inteded?");
        }
    }

    public static void Undo(IStagedAction action)
    {
        // If we're not in a check, we don't care.
        if (CheckContext == null) return;

        if (CheckContext.StagedActions.Remove(action))
        {
            if (!action.IsFreely)
            {
                if (!CheckContext.StagedCardTypes.Remove(action.CardData.cardType))
                    Debug.LogError($"{action.CardData.cardName} attempted to undo its type without being staged!");
            }
        }
        else
        {
            Debug.LogError($"{action.CardData.cardName} attempted to undo without being staged!");
        }
    }

    // --- Logic ---
    public static IPlayableLogic GetPlayableLogic(CardData cardData) => ServiceLocator.Get<LogicRegistry>().GetPlayableLogic(cardData);
    public static IEncounterLogic GetEncounterLogic(CardData cardData) => ServiceLocator.Get<LogicRegistry>().GetEncounterLogic(cardData.cardID);
}
