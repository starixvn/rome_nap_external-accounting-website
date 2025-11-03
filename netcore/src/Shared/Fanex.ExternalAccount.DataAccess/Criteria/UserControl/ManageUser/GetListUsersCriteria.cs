namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.UserCRUD
{
    using Fanex.Data.Repository;

    public class GetListUsersCriteria : ICriteria
    {
        public GetListUsersCriteria(string userName, int? groupId)
        {
            Username = userName;
            GroupID = groupId;
        }
        
        public string Username { get; set; }

        public int? GroupID { get; set; }

        public string GetSettingKey() => "EAS_UserInfoSel";

        public bool IsValid() => true;
    }
}