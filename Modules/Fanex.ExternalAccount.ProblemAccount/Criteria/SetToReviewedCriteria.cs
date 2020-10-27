namespace Fanex.ExternalAccount.ProblemAccount.Criteria
{
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.ProblemAccount.ViewModels;
    using Fanex.NAP.Framework.Data;

    public class SetToReviewedCriteria : CriteriaBase
    {
        public SetToReviewedCriteria(SetToReviewedViewModel viewModel)
        {
            CustomerList = viewModel.CustIds;
            RoleId = viewModel.RoleId;
            GroupName = viewModel.GroupName;
            IsAffectedAllDownlines = viewModel.IsAffectedAllDownlines;
            UserId = viewModel.UserId;
            IP = viewModel.IP;
        }

        public string CustomerList { get; }

        public byte RoleId { get; }

        public string GroupName { get; }

        public bool IsAffectedAllDownlines { get; }

        public int UserId { get; }

        public string IP { get; }

        public override string GetSettingKey()
            => "EAS_ProblemAccount_ReviewStatusUpd";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(CustomerList)
            && RoleId < Role.Super
            && !string.IsNullOrWhiteSpace(GroupName)
            && UserId > 0;
    }
}
