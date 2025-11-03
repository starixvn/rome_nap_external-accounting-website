namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using Fanex.Data.Repository;

    public class GetCustomerInfoCriteria : CriteriaBase
    {
        public GetCustomerInfoCriteria(string userName,string groupName, int userId)
        {
            UserName = userName;
            GroupName = groupName;
            UserId = userId;
        }

        public string UserName { get; }

        public string GroupName { get; }

        public int UserId { get; }

        public override string GetSettingKey()
            => "EAS_CustomerSelAllInfo";

        public override bool IsValid()
            => UserId > 0 && !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(GroupName);
    }
}
