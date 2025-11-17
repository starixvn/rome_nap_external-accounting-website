namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.EditSuperList
{
    using Fanex.Data;
    using Fanex.ExternalAccount.Core.Shared;

    public class UpdateSuperHideBetIPCommand : INonQueryBaseCommand
    {
        public int CustId { get; set; }

        [SpParam(SqlParamName = "IsHideIP")]
        public bool IsHideBetIP { get; set; }

        public string IP { get; set; }

        public int UserId { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public string GetSettingKey()
         => "EAS_CustomerUpdIsHideIP";

        public bool IsValid() => CustId > 0 && UserId > 0;
    }
}
