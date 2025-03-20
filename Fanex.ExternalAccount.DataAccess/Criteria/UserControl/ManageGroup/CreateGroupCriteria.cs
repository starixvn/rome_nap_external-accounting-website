namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.Group
{
    using Fanex.Data.Repository;

    public class CreateGroupCriteria : INonQueryCommand
    {
        public int UserId { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public int ParentId { get; set; }
        public string IP { get; set; }

        // Output parameters
        public int GroupId { get; set; }
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }

        public string GetSettingKey() => "EAS_GroupInfoIns";

        public bool IsValid() => UserId > 0 && !string.IsNullOrEmpty(GroupName) && !string.IsNullOrEmpty(IP);
    }
} 