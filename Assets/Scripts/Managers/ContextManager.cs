using UnityEngine;

public class ContextManager : MonoBehaviour
{
    public GameContext GameContext { get; set; } = null;
    public TurnContext TurnContext { get; set; } = null;
    public EncounterContext EncounterContext { get; set; } = null;
    public ActionContext ActionContext { get; set; } = null;

    private void Awake()
    {
        ServiceLocator.Register(this);

        Game.Initialize(this);
    }
}
