namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetCustomerInfoCriteria : CriteriaBase
    {
        public GetCustomerInfoCriteria(string groupName, int custId, string userName, bool isGetUplineInfo)
        {
            this.GroupName = groupName;
            this.CustId = custId;
            this.UserName = userName;
            this.IsGetUplineInfo = IsGetUplineInfo;
        }

        public string GroupName { get; }

        public int CustId { get; }

        public string UserName { get; }

        public bool IsGetUplineInfo { get; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "EAS_CustomerSelUserInfo";
    }
}
