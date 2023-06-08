using Fanex.NAP.Framework.Data;

namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.MarkAccountType
{
    public class AddMarkAccountCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public string Usernames { get; set; }

        public string GroupName { get; set; }

        public int TypeId { get; set; }

        public string Remark { get; set; }

        public string IP { get; set; }

        public string ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
         => "EAS_AccountType_Multiple_Ins";

        public override bool IsValid()
            => UserId > 0 && !string.IsNullOrEmpty(GroupName);
    }
}
