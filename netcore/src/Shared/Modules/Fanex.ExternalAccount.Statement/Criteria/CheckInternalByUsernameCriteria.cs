namespace Fanex.ExternalAccount.Statement.Criteria
{
    using Fanex.Data.Repository;

    public class CheckInternalByUsernameCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public bool IsInternal { get; set; }

        public override string GetSettingKey()
        {
            return "Statement_Report_CheckInternalByUsername";
        }

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(Username);
    }
}
