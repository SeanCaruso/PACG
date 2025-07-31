using UnityEngine;

public static class Game
{
    public static void Initialize(object manager)
    {
        if (manager is ContextManager contextManager) _contextManager = contextManager;
        if (manager is CardManager cardManager) _cardManager = cardManager;
    }

    // --- Common card functionality ---
    private static CardManager _cardManager;
    public static CardManager CardManager => _cardManager;
    public static void MoveCard(CardInstance card, CardLocation newLoc) => _cardManager.MoveCard(card, newLoc);

    // --- Contexts ---
    private static ContextManager _contextManager;

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
                if (CheckContext.StagedCardTypes.Contains(action.Card.Data.cardType)) Debug.Log($"{action.Card.Data.cardName} staged a duplicate type - was this intended?");
                CheckContext.StagedCardTypes.Add(action.Card.Data.cardType);
            }
        }
        else
        {
            Debug.Log($"{action.Card.Data.cardName}.{action} staged multiple times - was this inteded?");
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
                if (!CheckContext.StagedCardTypes.Remove(action.Card.Data.cardType))
                    Debug.LogError($"{action.Card.Data.cardName} attempted to undo its type without being staged!");
            }
        }
        else
        {
            Debug.LogError($"{action.Card.Data.cardName} attempted to undo without being staged!");
        }
    }

    // --- Logic ---
    public static IPlayableLogic GetPlayableLogic(CardInstance card) => ServiceLocator.Get<LogicRegistry>().GetPlayableLogic(card);
    public static IEncounterLogic GetEncounterLogic(CardInstance card) => ServiceLocator.Get<LogicRegistry>().GetEncounterLogic(card);
}
