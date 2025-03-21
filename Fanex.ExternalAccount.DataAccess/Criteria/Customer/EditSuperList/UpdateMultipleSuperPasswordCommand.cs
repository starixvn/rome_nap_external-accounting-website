namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.EditSuperList
{
    using Fanex.ExternalAccount.Core.Shared;

    public class UpdateMultipleSuperPasswordCommand : INonQueryBaseCommand
    {
        public string CustIDList { get; set; }

        public string UserPwd { get; set; }

        public string Language { get; set; }

        public string SubAccID { get; set; }

        public string LoginIP { get; set; }

        public int UserId { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public string GetSettingKey()
         => "Age_CustomerUpdPwd_Multiple";

        public bool IsValid() => true;
    }
}
