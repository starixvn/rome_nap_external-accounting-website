namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using Fanex.Data.Repository;

    public class GetUserInfoCriteria : CriteriaBase
    {
        public int CustId { get; }
        public byte SettingStatus { get; }

        public GetUserInfoCriteria(int custId, byte settingStatus)
        {
            CustId = custId;
            SettingStatus = settingStatus;
        }

        public override string GetSettingKey() => "Age_CustomerSelUserInfo";

        public override bool IsValid() => CustId > 0 && SettingStatus >= 0;
    }
}
