namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using Fanex.Data.Repository;

    public class IsArchivedCustIdCommand : INonQueryCommand
    {
        public int CustId { get; }
        public bool IsArchived { get; set; }

        public IsArchivedCustIdCommand(int custId)
        {
            CustId = custId;
        }

        public string GetSettingKey() => "Acc_GetCustArchiveStatus";

        public bool IsValid() => CustId > 0;
    }
}