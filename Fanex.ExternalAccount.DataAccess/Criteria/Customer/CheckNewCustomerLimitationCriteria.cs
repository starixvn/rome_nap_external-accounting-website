namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using Fanex.NAP.Framework.Data;

    public class CheckNewCustomerLimitationCriteria : CriteriaBase
    {
        public int RoleId { get; set; }

        public int CustId { get; set; }

        public string Language { get; set; } = "E";

        [SpParam(SqlParamName = "prefix")]
        public string Username { get; set; }

        public string ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
            => "Age_CustomerCheckReachDownline";

        public override bool IsValid()
            => CustId > 0 && RoleId > 0 && RoleId < 5;
    }
}
