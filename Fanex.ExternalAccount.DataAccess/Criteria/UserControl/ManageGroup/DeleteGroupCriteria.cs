namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.Group
{
    using Fanex.Data.Repository;

    public class DeleteGroupCriteria : INonQueryCommand
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }

        // Output parameters
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }

        public string GetSettingKey() => "EAS_GroupInfoDel";

        public bool IsValid() => GroupId > 0 && UserId > 0;
    }
}