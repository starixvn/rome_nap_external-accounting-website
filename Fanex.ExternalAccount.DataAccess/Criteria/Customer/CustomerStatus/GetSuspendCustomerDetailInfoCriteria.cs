namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerStatus
{
    using Fanex.NAP.Framework.Data;

    public class GetSuspendCustomerDetailInfoCriteria : CriteriaBase
    {
        public GetSuspendCustomerDetailInfoCriteria(
            int custId,
            bool isAffected,
            int downlineRoleId,
            int pageSize = 50,
            int pageIndex = 0)
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

        public override string GetSettingKey()
        {
            return "Acc_Customer_SuspendInfoDetailSel";
        }

        public override bool IsValid()
        {
            return CustId > 0;
        }
    }
}
