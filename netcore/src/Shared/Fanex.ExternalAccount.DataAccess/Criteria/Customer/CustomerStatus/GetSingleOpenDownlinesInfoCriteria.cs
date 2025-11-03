namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerStatus
{
    using Core.Shared;
    using Fanex.Data.Repository;

    public class GetSingleOpenDownlinesInfoCriteria : CriteriaBase
    {
        public GetSingleOpenDownlinesInfoCriteria(
            int custId,
            bool isAffected,
            int downlineRoleId,
            int pageSize = PagingConstants.DefaultPageSize,
            int pageIndex = PagingConstants.DefaultPageIndex)
        {
            CustId = custId;
            IsAffected = isAffected;
            DownlineRoleId = downlineRoleId;
            PageSize = pageSize;
            PageIndex = pageIndex;
        }

        public int CustId { get; }

        public bool IsAffected { get; }

        public int DownlineRoleId { get; }

        public int PageSize { get; }

        public int PageIndex { get; }

        public int TotalRows { get; set; }

        public override string GetSettingKey() => "Acc_Customer_CloseInfoDetailSel";

        public override bool IsValid() => CustId > 0;
    }
}