namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using Fanex.Data.Repository;

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

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey() => "EAS_CustomerInfo_MultiCheck";

        public override bool IsValid() => IsCopy ?
            !string.IsNullOrWhiteSpace(SourceUser) && !string.IsNullOrWhiteSpace(DestUsers) :
             !string.IsNullOrWhiteSpace(DestUsers);
    }
}