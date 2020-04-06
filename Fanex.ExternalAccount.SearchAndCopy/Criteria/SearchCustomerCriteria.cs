namespace Fanex.ExternalAccount.SearchAndCopy.Criteria
{
    using Fanex.NAP.Framework.Data;

    public class SearchCustomerCriteria : CriteriaBase
    {
        public SearchCustomerCriteria(string username, string groupName)
        {
            Username = username;
            GroupName = groupName;
        }

        public string Username { get; }

        public string GroupName { get; }

        public override string GetSettingKey() => "EAS_CustomerUplineUserNameSel";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(GroupName);
    }
}
