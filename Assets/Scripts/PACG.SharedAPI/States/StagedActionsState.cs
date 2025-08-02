namespace PACG.SharedAPI.States
{
    public class StagedActionsState
    {
        public readonly bool IsCancelButtonVisible;
        public readonly bool IsCommitButtonVisible;
        public readonly bool UseSkipSprite;

        public StagedActionsState(bool isCancelButtonVisible, bool isCommitButtonVisible, bool useSkipSprite)
        {
            IsCancelButtonVisible = isCancelButtonVisible;
            IsCommitButtonVisible = isCommitButtonVisible;
            UseSkipSprite = useSkipSprite;
        }
    }
}