namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.Group
{
    using Fanex.Data.Repository;

    public class EditGroupPolicyCriteria : INonQueryCommand
    {
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public string OldGroupPolicy { get; set; }
        public string GroupPolicy { get; set; }
        public string IP { get; set; }

        // Output parameters
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }

        public string GetSettingKey() => "EAS_GroupPolicyUpd";

        public bool IsValid() => UserId > 0 && GroupId > 0 && !string.IsNullOrEmpty(IP);
    }
} 