namespace Fanex.ExternalAccount.ProblemAccount.ViewModels
{
    using System;
    using Fanex.ExternalAccount.Core.Context;

    public class GetListViewModel
    {
        public string GroupName
            => ExtRequest.Instance.GetGroupName();

        public byte SearchRoleId { get; set; }

        public string UplineName { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public byte Status { get; set; }

        public string Query { get; set; }
    }
}
