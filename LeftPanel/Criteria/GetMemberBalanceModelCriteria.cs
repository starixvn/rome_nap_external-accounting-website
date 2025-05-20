namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DTO;

    public class GetMemberBalanceModelCommand : INonQueryCommand
    {
        public GetMemberBalanceModelCommand(int custId, SiteDB siteDB)
        {
            CustId = custId;
            SiteDB = siteDB;
        }

        public int CustId { get; }
        public SiteDB SiteDB { get; }

        public decimal Balance { get; set; }
        public decimal PendingTransfer3rd { get; set; }
        public decimal NetPosition { get; set; }

        public string GetSettingKey()
        {
            var prefixSp = SiteDB.IsSiteDB ? $"SiteDB{SiteDB.SiteDBGroupID}_" : string.Empty;
            return $"{prefixSp}Acc_BalanceMemberSel";
        }

        public bool IsValid() => true;
    }
}
