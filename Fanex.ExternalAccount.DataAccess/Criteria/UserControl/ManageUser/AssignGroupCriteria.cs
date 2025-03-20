namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.UserCRUD
{
    using Fanex.Data.Repository;

    public class AssignGroupCriteria : ResultCriteria, INonQueryCommand
    {
        public AssignGroupCriteria(int userId, int groupId, int creator)
        {
            UserId = userId;
            GroupId = groupId;
            Creator = creator;
        }
        public int UserId { get; set; }
        
        public int GroupId { get; set; }
        
        public int Creator { get; set; }
        
        public string GetSettingKey() => "EAS_eaUserInfo_Ins";

        public bool IsValid() => true;
    }
}