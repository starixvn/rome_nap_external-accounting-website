namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.EditSuperList
{
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.Core.Shared;

    public class UpdateMultipleSuperSecurityCodeCommand : INonQueryBaseCommand
    {
        public string CustIDList { get; set; }

        public bool IsForcedChangeSecurityCode { get; set; }

        public string Language { get; set; }

        public string System { get; set; }

        public string IP { get; set; }

        public int UserId { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public string GetSettingKey()
         => "Acc_CustomerUpdSecurityCode_Multiple";

        public bool IsValid() => !string.IsNullOrWhiteSpace(CustIDList) && UserId > 0;
    }
}
