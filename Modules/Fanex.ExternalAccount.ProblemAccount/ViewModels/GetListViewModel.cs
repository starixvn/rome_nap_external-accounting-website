namespace Fanex.ExternalAccount.ProblemAccount.ViewModels
{
    using System;
    using Fanex.ExternalAccount.Core.Context;

    public class GetListViewModel
    {
        public string GroupName
            => ExtRequest.Instance.GetGroupName();

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string UplineUsername { get; set; }

        public byte SearchRoleId { get; set; }

        public string SearchUsername { get; set; }

        public byte ReviewStatus { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
