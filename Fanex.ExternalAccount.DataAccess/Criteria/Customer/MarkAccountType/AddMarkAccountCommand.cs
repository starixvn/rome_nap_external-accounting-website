namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.MarkAccountType
{
    using Fanex.Data.Repository;

    public class AddMarkAccountCommand : INonQueryCommand
    {
        public int UserId { get; set; }

        public string Usernames { get; set; }

        public string GroupName { get; set; }

        public int TypeId { get; set; }

        public string Remark { get; set; }

        public string IP { get; set; }

        public string ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public string GetSettingKey()
         => "EAS_AccountType_Multiple_Ins";

        public bool IsValid()
            => UserId > 0 && !string.IsNullOrEmpty(GroupName);
    }
}
