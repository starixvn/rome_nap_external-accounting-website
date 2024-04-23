using Fanex.NAP.Framework.Data;

namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    public class CheckMultipleSupersBetSettingCriteria : CriteriaBase
    {
        public CheckMultipleSupersBetSettingCriteria(int userId, string sourceUser, string destUsers, string groupName, bool isCopy)
        {
            UserId = userId;
            SourceUser = sourceUser;
            DestUsers = destUsers;
            GroupName = groupName;
            IsCopy = isCopy;
        }

        public int UserId { get; }

        public string SourceUser { get; }

        public string DestUsers { get; }

        public string GroupName { get; }

        public bool IsCopy { get; }

        public override string GetSettingKey() => "EAS_CustomerInfo_MultiCheck";

        public override bool IsValid() => IsCopy ?
            !string.IsNullOrWhiteSpace(SourceUser) && !string.IsNullOrWhiteSpace(DestUsers) :
             !string.IsNullOrWhiteSpace(DestUsers);
    }
}