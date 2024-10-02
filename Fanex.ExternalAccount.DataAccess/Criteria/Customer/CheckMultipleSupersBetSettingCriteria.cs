using Fanex.NAP.Framework.Data;

namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    public class CheckMultipleSupersBetSettingCriteria : CriteriaBase
    {
        public CheckMultipleSupersBetSettingCriteria(string sourceUser, string destUsers, string groupName, bool isCopy)
        {
            SourceUser = sourceUser;
            DestUsers = destUsers;
            GroupName = groupName;
            IsCopy = isCopy;
        }

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