namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerStatus
{
    using Fanex.NAP.Framework.Data;

    public class UpdateSuspendCustomerCriteria : CriteriaBase
    {
        public int UserId { get; }

        public int CustId { get; }

        public bool Suspended { get; }

        public string Language { get; }

        public string IP { get; }

        public UpdateSuspendCustomerCriteria(int userId, int custId, bool suspended, string language, string ip)
        {
            UserId = userId;
            CustId = custId;
            Suspended = suspended;
            Language = language;
            IP = ip;
        }

        public override string GetSettingKey()
        {
            return "Acc_CustomerUpdSuspended";
        }

        public override bool IsValid()
        {
            return UserId > 0 && CustId > 0;
        }
    }
}
