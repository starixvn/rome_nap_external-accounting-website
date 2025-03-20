namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.UserCRUD
{
    using Fanex.Data.Repository;

    public class WriteLogCriteria : INonQueryCommand
    {
        public int CustId { get; set; }
        
        public int UserId { get; set; }
       
        public string TrackingValue { get; set; }
        
        public int TypeID { get; set; }
        
        public int ActionID { get; set; }
        
        public int Duration { get; set; }
        
        public string GetSettingKey() => "EAS_WriteLog_UserLog_UserControl_Wrapper";

        public bool IsValid() => CustId > 0 && UserId > 0;
    }
}