namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.MarkAccountType
{
    using Fanex.Data.Repository;

    public class UnmarkMarkAccountCommand : INonQueryCommand
    {
        public int CustID { get; set; }

        public int UserID { get; set; }

        public string IP { get; set; }

        public string ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public string GetSettingKey()
         => "EAS_AccountType_Del";

        public bool IsValid()
            => CustID > 0 && UserID > 0;
    }
}
