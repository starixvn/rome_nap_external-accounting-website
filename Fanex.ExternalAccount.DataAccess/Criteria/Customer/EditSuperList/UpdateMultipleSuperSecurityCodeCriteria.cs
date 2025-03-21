namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.EditSuperList
{
    using Fanex.Data.Repository;

    public class UpdateMultipleSuperSecurityCodeCriteria : CriteriaBase
    {
        public string CustIDList { get; set; }

        public bool IsForcedChangeSecurityCode { get; set; }

        public string Language { get; set; }

        public string System { get; set; }

        public string IP { get; set; }

        public int UserId { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
         => "Acc_CustomerUpdSecurityCode_Multiple";

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(CustIDList) && UserId > 0;
        }
    }
}
