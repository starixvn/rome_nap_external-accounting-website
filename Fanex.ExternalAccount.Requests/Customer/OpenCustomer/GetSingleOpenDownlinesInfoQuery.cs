namespace Fanex.ExternalAccount.Requests.Customer.OpenCustomer
{
    using Core.Shared;

    public class GetSingleOpenDownlinesInfoQuery
    {
        public int CustId { get; set; }

        public bool IsAffected { get; set; }

        public int DownlineRoleId { get; set; }

        public int PageSize { get; set; } = PagingConstants.DefaultPageSize;

        public int PageIndex { get; set; } = PagingConstants.DefaultPageIndex;
    }
}