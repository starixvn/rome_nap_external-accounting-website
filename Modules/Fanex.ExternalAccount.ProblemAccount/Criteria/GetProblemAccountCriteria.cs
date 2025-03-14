namespace Fanex.ExternalAccount.ProblemAccount.Criteria
{
    using System;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.ProblemAccount.ViewModels;

    public class GetProblemAccountCriteria : CriteriaBase
    {
        public GetProblemAccountCriteria(GetListViewModel viewModel)
        {
            FromDate = viewModel.FromDate;
            ToDate = viewModel.ToDate;
            GroupName = viewModel.GroupName;
            UplineUsername = viewModel.UplineUsername;
            SearchRoleId = viewModel.SearchRoleId;
            SearchUsername = viewModel.SearchUsername;
            ReviewStatusId = viewModel.ReviewStatus;
            PageIndex = viewModel.PageIndex;
            PageSize = viewModel.PageSize;
        }

        public DateTime FromDate { get; }

        public DateTime ToDate { get; }

        public string GroupName { get; }

        public string UplineUsername { get; }

        public byte SearchRoleId { get; }

        public string SearchUsername { get; }

        public byte ReviewStatusId { get; }

        public int PageIndex { get; }

        public int PageSize { get; }

        public int TotalRows { get; set; }

        public override string GetSettingKey()
            => "EAS_ProblemAccount_CustomerList";

        public override bool IsValid()
            => FromDate <= ToDate
            && !string.IsNullOrWhiteSpace(GroupName)
            && SearchRoleId < Role.Super;
    }
}
