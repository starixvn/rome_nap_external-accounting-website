namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.EditSuperList
{
    using Fanex.ExternalAccount.Core.Shared;

    public class UpdateMultipleSuperInfoCommand : INonQueryBaseCommand
    {
        public string CustIDList { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string MobilePhone { get; set; }

        public string IP { get; set; }

        public int UserId { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public string GetSettingKey()
         => "EAS_CustomerUpdSuperInfo_Multiple";

        public bool IsValid() => true;
    }
}
