namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.EditSuperList
{
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.Core.Shared;

    public class UpdateMultipleSuperHideBetIPCommand : INonQueryBaseCommand
    {
        public string CustIDList { get; set; }

        public bool IsHideBetIP { get; set; }

        public string IP { get; set; }

        public int UserId { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public string GetSettingKey()
         => "EAS_CustomerUpdIsHideIP_Multiple";

        public bool IsValid() => !string.IsNullOrWhiteSpace(CustIDList) && UserId > 0;
    }
}
