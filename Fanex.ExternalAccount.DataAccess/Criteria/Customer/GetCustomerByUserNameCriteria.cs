namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using Fanex.Data.Repository;

    public class GetCustomerByUserNameCriteria : CriteriaBase
    {
        public string UserName { get; }
        public string GroupName { get; }
        public int UserId { get; }

        public GetCustomerByUserNameCriteria(string userName, string groupName, int userId)
        {
            UserName = userName;
            GroupName = groupName;
            UserId = userId;
        }

        public override string GetSettingKey() => "EAS_CustomerSelAllInfo";

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(UserName) && 
                   !string.IsNullOrEmpty(GroupName) && 
                   UserId > 0;
        }
    }
}
