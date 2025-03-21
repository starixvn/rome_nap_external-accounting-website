namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.EditSuperList
{
    using Fanex.Data.Repository;

    public class UpdateMultipleSuperPasswordCirteria : CriteriaBase
    {
        public string CustIDList { get; set; }

        public string UserPwd { get; set; }

        public string Language { get; set; }

        public string SubAccID { get; set; }

        public string LoginIP { get; set; }

        public int UserId { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
         => "Age_CustomerUpdPwd_Multiple";

        public override bool IsValid() => true;
    }
}
