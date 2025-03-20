namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.MarkAccountType
{
    using Fanex.Data.Repository;

    public class UpdateMarkAccountCommand : INonQueryCommand
    {
        public int CustID { get; set; }

        public int TypeID { get; set; }

        public string Remark { get; set; }
        
        public int UserID { get; set; }

        public string IP { get; set; }

        public string ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public string GetSettingKey()
         => "EAS_AccountType_Upd";

        public bool IsValid()
            => CustID > 0 && UserID > 0;
    }
}
