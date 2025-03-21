namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using Fanex.Data.Repository;

    public class CheckLimitNewDownlineCommand : INonQueryCommand
    {
        public CheckLimitNewDownlineCommand(int custId)
        {
            CustId = custId;
        }

        public int CustId { get; }

        public bool IsAllowed { get; set; }

        public string GetSettingKey()
            => "Acc_ProblemAccountLimitDownlineCheck";

        public bool IsValid()
            => CustId > 0;
    }
}
