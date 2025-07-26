using UnityEngine;

public class ContextManager : MonoBehaviour
{
    public GameContext GameContext {  get; set; }
    public TurnContext TurnContext { get; set; }
    public EncounterContext EncounterContext { get; set; }
    public ActionContext ActionContext { get; set; }

    private void Awake()
    {
        Game.Initialize(this);
    }
}
