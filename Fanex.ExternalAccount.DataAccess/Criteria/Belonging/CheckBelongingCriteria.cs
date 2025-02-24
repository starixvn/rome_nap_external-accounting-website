namespace Fanex.ExternalAccount.DataAccess.Criteria.Belonging
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CheckBelongingCriteria : CriteriaBase
    {
        public CheckBelongingCriteria(
            int userId,
            string groupName
          )
        {
            UserId = userId;
            GroupName = groupName;
        }

        public int UserId { get; }

        public string GroupName { get; }

        public int ErrCode { get; }

        public string ErrMsg { get; }

        public override string GetSettingKey()
            => "EAS_UserCheckBelonging";

        public override bool IsValid()
            => UserId > 0 && !string.IsNullOrEmpty(GroupName);
    }
}
