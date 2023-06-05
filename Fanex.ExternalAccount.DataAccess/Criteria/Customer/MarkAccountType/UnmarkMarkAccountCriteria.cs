namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.MarkAccountType
{
    using Fanex.NAP.Framework.Data;

    public class UnmarkMarkAccountCriteria : CriteriaBase
    {
        public int CustID { get; set; }

        public int UserID { get; set; }

        public string IP { get; set; }

        public string ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
         => "EAS_AccountType_Del";

        public override bool IsValid()
            => CustID > 0 && UserID > 0;
    }
}
