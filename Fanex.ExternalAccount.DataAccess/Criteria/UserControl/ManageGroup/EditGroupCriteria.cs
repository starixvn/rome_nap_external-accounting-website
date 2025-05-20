namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.Group
{
    using Fanex.Data.Repository;

    public class EditGroupCriteria : INonQueryCommand
    {
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public string IP { get; set; }

        // Output parameters
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }

        public string GetSettingKey() => "EAS_GroupInfoUpd";

        public bool IsValid() => UserId > 0 && GroupId > 0 && !string.IsNullOrEmpty(GroupName) && !string.IsNullOrEmpty(IP);
    }
} 