namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.EditSuperList
{
    using Fanex.NAP.Framework.Data;

    public class CheckMultipleSupersCriteria : CriteriaBase
    {
        public CheckMultipleSupersCriteria(int userId, string destUsers, string groupName)
        {
            UserId = userId;
            DestUsers = destUsers;
            GroupName = groupName;
        }

        public int UserId { get; }

        public string SourceUser { get; }

        public string DestUsers { get; }

        public string GroupName { get; }

        public bool IsCopy { get; }

        public override string GetSettingKey() => "EAS_CustomerInfo_MultiCheck";
    }
}
