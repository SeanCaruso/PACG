
namespace PACG.SharedAPI
{
    public class StagedActionsState
    {
        public readonly bool IsCancelButtonVisible;
        public readonly bool IsCommitButtonVisible;
        public readonly bool IsSkipButtonVisible;
        public readonly bool IsExploreEnabled;

        public StagedActionsState(bool canCancel, bool canCommit, bool canSkip, bool isExploreEnabled)
        {
            IsCancelButtonVisible = canCancel;
            IsCommitButtonVisible = canCommit;
            IsSkipButtonVisible = canSkip;
            IsExploreEnabled = isExploreEnabled;

        }
    }
}