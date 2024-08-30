namespace Fanex.ExternalAccount.SearchAndCopy.Criteria
{
    using Fanex.NAP.Framework.Data;

    public class SearchCustomerCriteria : CriteriaBase
    {
        public SearchCustomerCriteria(string username, string groupName, int custDBGroupId)
        {
            Username = username;
            GroupName = groupName;
            SiteDBGroupId = custDBGroupId;
        }

        public string Username { get; }

        public string GroupName { get; }

        [SpParam(Ignored = true)]
        public int SiteDBGroupId { get; set; }

        public override string GetSettingKey() => (SiteDBGroupId == 0 ? string.Empty : $"DBSite{SiteDBGroupId}_") + "EAS_CustomerUplineUserNameSel";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(GroupName);
    }
}
