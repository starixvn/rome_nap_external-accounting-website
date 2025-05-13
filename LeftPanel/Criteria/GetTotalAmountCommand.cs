namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DTO;

    public class GetTotalAmountCommand : INonQueryCommand
    {
        public int CustId { get; }
        public SiteDB SiteDB { get; }
        public decimal TotalAmt { get; set; }

        public GetTotalAmountCommand(int custId, SiteDB siteDB)
        {
            CustId = custId;
            SiteDB = siteDB;
        }

        public string GetSettingKey()
        {
            var prefixSp = SiteDB.IsSiteDB ? $"SiteDB{SiteDB.SiteDBGroupID}_" : string.Empty;
            return $"{prefixSp}Acc_CustomerTotalAmountSel";
        }

        public bool IsValid()
        {
            return CustId > 0;
        }
    }
}
