namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetSuggestUsernamesCriteria : CriteriaBase
    {
        public GetSuggestUsernamesCriteria(int custId, string userName, string groupName)
        {
            this.CustId = custId;
            this.UserName = userName;
            this.GroupName = groupName;
        }

        public int CustId { get; }

        public string UserName { get; }

        public string GroupName { get; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "EAS_CustomerSelUserSearch";
    }
}
