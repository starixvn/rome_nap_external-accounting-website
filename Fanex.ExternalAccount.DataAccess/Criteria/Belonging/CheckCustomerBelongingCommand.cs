namespace Fanex.ExternalAccount.DataAccess.Criteria.Belonging
{
    using Fanex.Data.Repository;

    public class CheckCustomerBelongingCommand : INonQueryCommand
    {
        public CheckCustomerBelongingCommand(
            string groupName,
            int custId,
            string username)
        {
            GroupName = groupName;
            CustId = custId;
            Username = username;
        }

        public string GroupName { get; }

        public int CustId { get; }

        public string Username { get; }

        public bool IsBelonging { get; set; }

        public string GetSettingKey()
            => "EAS_CustomerSelCheckBelonging";

        public bool IsValid()
            => !string.IsNullOrEmpty(Username) || CustId > 0;
    }
}
