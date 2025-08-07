namespace PACG.SharedAPI
{
    public class StagedActionsState
    {
        public readonly bool IsCancelButtonVisible;
        public readonly bool IsCommitButtonVisible;
        public readonly bool IsSkipButtonVisible;

        public StagedActionsState(bool canCancel, bool canCommit, bool canSkip)
        {
            IsCancelButtonVisible = canCancel;
            IsCommitButtonVisible = canCommit;
            IsSkipButtonVisible = canSkip;
        }
    }
}