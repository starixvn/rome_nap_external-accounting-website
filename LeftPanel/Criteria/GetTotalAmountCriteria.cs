namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DTO;

    public class GetTotalAmountCriteria : CriteriaBase
    {
        public int CustId { get; }
        public SiteDB SiteDB { get; }

        public GetTotalAmountCriteria(int custId, SiteDB siteDB)
        {
            CustId = custId;
            SiteDB = siteDB;
        }

        public override string GetSettingKey()
        {
            var prefixSp = SiteDB.IsSiteDB ? $"SiteDB{SiteDB.SiteDBGroupID}_" : string.Empty;
            return $"{prefixSp}Acc_CustomerTotalAmountSel";
        }

        public override bool IsValid()
        {
            return CustId > 0;
        }
    }
}
