namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data.Repository;

    public class GetMemberOutstandingCommand : INonQueryCommand
    {
        public GetMemberOutstandingCommand(int custId)
        {
            CustId = custId;
        }

        public int CustId { get; }

        public decimal Outstanding { get; set; }

        public string GetSettingKey() => "Acc_OutstandingMemberSel";

        public bool IsValid() => CustId > 0;
    }
}