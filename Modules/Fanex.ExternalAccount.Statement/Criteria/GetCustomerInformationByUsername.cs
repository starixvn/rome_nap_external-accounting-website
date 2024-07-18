namespace Fanex.ExternalAccount.Statement.Criteria
{
    using CustomerSetting.Module.Core.Web;
    using Fanex.CustomerSetting.Module.Core;
    using Fanex.NAP.Framework.Data;
    using System.Configuration;

    public class GetCustomerInformationByUsername : CriteriaBase
    {
        public string Username { get; set; }

        public int UserId
            => AppSession.CurrentSession.UserId;

        public byte SystemId 
            => (byte)Utils.ToInt32(ConfigurationManager.AppSettings["SystemId"]);

        public override string GetSettingKey()
            => "Statement_Report_GetCustomerInformationByUsername";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(Username) && UserId > 0;
    }
}
