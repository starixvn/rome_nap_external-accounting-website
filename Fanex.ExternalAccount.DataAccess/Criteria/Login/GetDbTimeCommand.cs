namespace Fanex.ExternalAccount.DataAccess.Criteria.Login
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetDbTimeCommand : INonQueryCommand
    {
        public DateTime LoginTime { get; set; }

        public GetDbTimeCommand()
        {
        }

        public string GetSettingKey() => "Age_LoginAdminVerifyTicket";

        public bool IsValid() => true;
    }
}