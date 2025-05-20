namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.EditSuperList
{
    using Fanex.Data.Repository;

    public class CheckMultipleSupersCriteria : CriteriaBase
    {
        public CheckMultipleSupersCriteria(string destUsers, string groupName)
        {
            DestUsers = destUsers;
            GroupName = groupName;
        }

        public string SourceUser { get; }

        public string DestUsers { get; }

        public string GroupName { get; }

        public bool IsCopy { get; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey() => "EAS_CustomerInfo_MultiCheck";

        public override bool IsValid() => true;
    }
}
