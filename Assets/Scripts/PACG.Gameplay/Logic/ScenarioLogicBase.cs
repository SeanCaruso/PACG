namespace PACG.Gameplay
{
    public class ScenarioLogicBase : ILogicBase
    {
        public virtual bool HasAvailableAction => false;

        public virtual void InvokeAction()
        {
        }
    }
}
