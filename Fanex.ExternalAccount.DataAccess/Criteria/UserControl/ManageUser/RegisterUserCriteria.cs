namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.UserCRUD
{
    using Fanex.Data.Repository;

    public class RegisterUserCriteria : ResultCriteria, INonQueryCommand
    {
        public RegisterUserCriteria(string userName, int adminSiteDBGroupId, int roleId)
        {
            UserName = userName;
            AdminSiteDBGroupId = adminSiteDBGroupId;
            RoleId = roleId;
        }
        
        public string UserName { get; set; }
        
        public int AdminSiteDBGroupId { get; set; }
        
        public int RoleId { get; set; }
        
        public int CustId { get; set; }
        
        public string GetSettingKey() => "Acc_CustomerIns_Register";
        
        public bool IsValid() => true;
    }
}