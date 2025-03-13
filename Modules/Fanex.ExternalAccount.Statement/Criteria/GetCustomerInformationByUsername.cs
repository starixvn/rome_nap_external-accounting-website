namespace Fanex.ExternalAccount.Statement.Criteria
{
    using CustomerSetting.Module.Core.Web;
    using Fanex.CustomerSetting.Module.Core;
    using Fanex.Data.Repository;
    using System.Configuration;

    public class GetCustomerInformationByUsername : CriteriaBase
    {
        public string Username { get; set; }

        public override string GetSettingKey()
            => "Statement_Report_GetCustomerInformationByUsername";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(Username);
    }
}
