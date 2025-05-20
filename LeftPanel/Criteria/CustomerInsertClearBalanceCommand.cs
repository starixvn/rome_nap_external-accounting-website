namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DTO;

    public class CustomerInsertClearBalanceCommand : INonQueryCommand
    {
        public int FromCustId { get; }
        public string ToCustIds { get; }
        public int UserId { get; }
        public string Language { get; }
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }
        public string CusStatus { get; }
        public string Ip { get; }
        public string System { get; }
        public SiteDB SiteDB { get; }

        public CustomerInsertClearBalanceCommand(int fromCustId, string toCustIds, int userId, string language, string cusStatus, string ip, string system, SiteDB siteDB)
        {
            FromCustId = fromCustId;
            ToCustIds = toCustIds;
            UserId = userId;
            Language = language;
            CusStatus = cusStatus;
            Ip = ip;
            System = system;
            SiteDB = siteDB;
        }

        public string GetSettingKey()
        {
            var prefixSp = SiteDB.IsSiteDB ? $"SiteDB{SiteDB.SiteDBGroupID}_" : string.Empty;
            return $"{prefixSp}Acc_CustomerInsClearBalance";
        }

        public bool IsValid()
        {
            return FromCustId > 0 && !string.IsNullOrEmpty(ToCustIds);
        }
    }
}
