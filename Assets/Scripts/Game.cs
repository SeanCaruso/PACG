using UnityEngine;

public static class Game
{
    private static ContextManager _contextManager;

    public static void Initialize(ContextManager contextManager)
    {
        _contextManager = contextManager;
    }

    public static GameContext GameContext => _contextManager.GameContext;
    public static TurnContext TurnContext => _contextManager.TurnContext;
    public static EncounterContext EncounterContext => _contextManager.EncounterContext;
    public static ActionContext ActionContext => _contextManager.ActionContext;
}
