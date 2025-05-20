namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System.Globalization;

    public class UpdateSuggestUsernameCriteria : INonQueryCommand
    {
        public UpdateSuggestUsernameCriteria(int custId, string userName, string groupName)
        {
            this.CustId = custId;
            this.UserName = userName.ToUpper(CultureInfo.InvariantCulture);
            this.GroupName = groupName;
        }

        [SpParam(SqlParamName = "userid")]
        public int CustId { get; }

        [SpParam(SqlParamName = "search")]
        public string UserName { get; }

        public string GroupName { get; }

        public bool IsValid() => true;

        public string GetSettingKey() => "EAS_CustomerUpdUserSearch";
    }
}
