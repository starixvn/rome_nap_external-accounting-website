namespace Fanex.ExternalAccount.DataAccess.Criteria.Belonging
{
    using Fanex.NAP.Framework.Data;

    public class CheckCustomerBelongingCriteria : CriteriaBase
    {
        public CheckCustomerBelongingCriteria(
            string groupName,
            int custId,
            string username)
        {
            GroupName = groupName;
            CustId = custId;
            Username = username;
        }

        public string GroupName { get; }

        public int CustId { get; }

        public string Username { get; }

        public override string GetSettingKey()
            => "EAS_UserCheckBelonging";

        public override bool IsValid()
            => !string.IsNullOrEmpty(Username) || CustId > 0;
    }
}
