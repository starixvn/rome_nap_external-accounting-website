namespace Fanex.ExternalAccount.DataAccess.Criteria.Belonging
{
    using Fanex.Data.Repository;

    public class CheckBelongingCommand : INonQueryCommand
    {
        public CheckBelongingCommand(
            int userId,
            string groupName
          )
        {
            UserId = userId;
            GroupName = groupName;
        }

        public int UserId { get; }

        public string GroupName { get; }

        public bool IsBelonging { get; set; }

        public int ErrCode { get; }

        public string ErrMsg { get; }

        public string GetSettingKey()
            => "EAS_UserCheckBelonging";

        public bool IsValid()
            => UserId > 0 && !string.IsNullOrEmpty(GroupName);
    }
}
