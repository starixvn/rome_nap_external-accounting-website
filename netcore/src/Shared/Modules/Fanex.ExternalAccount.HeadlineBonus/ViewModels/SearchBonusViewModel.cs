namespace Fanex.ExternalAccount.HeadlineBonus.ViewModels
{
    using System;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Core.Context;

    public class SearchBonusViewModel
    {
        public string GroupName
            => ExtRequest.Instance.GetGroupName();

        [AllowHtml]
        public string Member { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public byte? StatusId { get; set; }

        public bool? IsComplained { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; } = 500;
    }
}
