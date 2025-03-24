namespace Fanex.ExternalAccount.Statement.Criteria
{
    using Fanex.Data.Repository;

    public class GetCustomerInformationByUsername : CriteriaBase
    {
        public string Username { get; set; }

        public override string GetSettingKey()
            => "Statement_Report_GetCustomerInformationByUsername";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(Username);
    }
}
