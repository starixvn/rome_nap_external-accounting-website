namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.UserCRUD
{
    using Fanex.Data.Repository;

    public class GetUserFunctionCriteria : ICriteria
    {
        public int UserID { get; set; }
        
        public string GetSettingKey() => "EAS_UserPolicySel";

        public bool IsValid() => UserID > 0;
    }
}