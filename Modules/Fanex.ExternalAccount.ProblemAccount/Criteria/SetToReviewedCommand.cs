namespace Fanex.ExternalAccount.ProblemAccount.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.ProblemAccount.ViewModels;

    public class SetToReviewedCommand : INonQueryCommand
    {
        public SetToReviewedCommand() { }

        public SetToReviewedCommand(SetToReviewedViewModel viewModel)
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
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }

        public string GetSettingKey()
            => "EAS_ProblemAccount_ReviewStatusUpd";

        public bool IsValid()
            => !string.IsNullOrWhiteSpace(CustomerList)
            && RoleId < Role.Super
            && !string.IsNullOrWhiteSpace(GroupName)
            && UserId > 0;
    }
}
