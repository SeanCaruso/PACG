using UnityEngine;

public abstract class GameBehaviour : MonoBehaviour
{
    protected CardManager Cards;
    protected ContextManager Contexts;
    protected LogicRegistry Logic;

    void Awake()
    {
        ServiceLocator.Register(this);

        Cards = ServiceLocator.Get<CardManager>();
        Contexts = ServiceLocator.Get<ContextManager>();
        Logic = ServiceLocator.Get<LogicRegistry>();

        OnAwake();
    }

    protected virtual void OnAwake() { }
}
