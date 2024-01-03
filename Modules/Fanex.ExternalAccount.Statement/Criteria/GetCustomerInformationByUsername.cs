namespace Fanex.ExternalAccount.Statement.Criteria
{
    using CustomerSetting.Module.Core.Web;
    using Fanex.NAP.Framework.Data;

    public class GetCustomerInformationByUsername : CriteriaBase
    {
        public string Username { get; set; }

        public int UserId
            => AppSession.CurrentSession.UserId;

        public override string GetSettingKey()
            => "Statement_Report_GetCustomerInformationByUsername";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(Username) && UserId > 0;
    }
}
