namespace Fanex.ExternalAccount.ProblemAccount.Criteria
{
    using System;
    using Fanex.ExternalAccount.ProblemAccount.ViewModels;
    using Fanex.NAP.Framework.Data;

    public class GetProblemAccountCriteria : CriteriaBase
    {
        public GetProblemAccountCriteria(GetListViewModel viewModel)
        {
            UplineUserName = viewModel.UplineName;
            SearchRoleId = viewModel.SearchRoleId;
            FromDate = viewModel.From;
            ToDate = viewModel.To;
            ReviewStatusId = viewModel.Status;
            SearchUserName = viewModel.Query;
        }

        public int CustId { get; } = 1176070;

        public int SubAccId { get; } = 0;

        public string UplineUserName { get; }

        public byte SearchRoleId { get; }

        public DateTime FromDate { get; }

        public DateTime ToDate { get; }

        public int ReviewStatusId { get; }

        public string SearchUserName { get; }

        public override string GetSettingKey()
            => "Age_ProblemAccount_CustomerList";

        public override bool IsValid()
            => CustId > 0 &&
            SubAccId >= 0 &&
            SearchRoleId <= 3;
    }
}
