namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerStatus
{
    using Fanex.Data.Repository;

    public class UpdateSuspendCustomerCommand : INonQueryCommand
    {
        public int UserId { get; }

        public int CustId { get; }

        public bool Suspended { get; }

        public string Language { get; }

        public string IP { get; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public UpdateSuspendCustomerCommand(int userId, int custId, bool suspended, string language, string ip)
        {
            UserId = userId;
            CustId = custId;
            Suspended = suspended;
            Language = language;
            IP = ip;
        }

        public string GetSettingKey()
        {
            return "Acc_CustomerUpdSuspended";
        }

        public bool IsValid()
        {
            return UserId > 0 && CustId > 0;
        }
    }
}
