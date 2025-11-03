namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.UserCRUD
{
    using Fanex.Data.Repository;

    public class Acc_CustomerInsUserInfo_RollbackCriteria : INonQueryCommand
    {
        public Acc_CustomerInsUserInfo_RollbackCriteria(int custId) => CustId = custId;
        public int CustId { get; set; }

        public string GetSettingKey() => "Age_CustomerInsUserInfo_Rollback";

        public bool IsValid() => CustId > 0;
    }
    
    public class Acc_CustomerIns_RollbackCriteria : INonQueryCommand
    {
        public Acc_CustomerIns_RollbackCriteria(int custId) => CustId = custId;
        public int CustId { get; set; }
        
        public string GetSettingKey() => "Acc_CustomerIns_Rollback";
        
        public bool IsValid() => CustId > 0;
    }
}