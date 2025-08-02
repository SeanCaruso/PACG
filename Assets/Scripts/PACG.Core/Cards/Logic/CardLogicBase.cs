using PACG.Services.Game;

public abstract class CardLogicBase : ICardLogic
{
    public CardInstance Card { get; set; }

    // Dependency injection of services
    protected ContextManager Contexts { get; }
    protected LogicRegistry Logic { get; }

    protected CardLogicBase()
    {
        Contexts = ServiceLocator.Get<ContextManager>();
        Logic = ServiceLocator.Get<LogicRegistry>();
    }
}
