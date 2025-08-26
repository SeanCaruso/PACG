namespace PACG.Gameplay
{
    public interface IOnBeforeDiscardResponse
    {
        void OnBeforeDiscard(CardInstance sourceCard, DiscardEventArgs args);
    }
}
