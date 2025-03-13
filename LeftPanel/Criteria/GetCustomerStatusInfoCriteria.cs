namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data.Repository;

    public class GetCustomerStatusInfoCriteria : CriteriaBase
    {
        public int CustId { get; }

        public byte SettingStatus { get; }

        public GetCustomerStatusInfoCriteria(int custId, byte settingStatus)
        {
            CustId = custId;
            SettingStatus = settingStatus;
        }

        public override string GetSettingKey()
        {
            return "EAS_CustomerSelAllStatus";
        }

        public override bool IsValid()
        {
            return CustId > 0;
        }
    }
}