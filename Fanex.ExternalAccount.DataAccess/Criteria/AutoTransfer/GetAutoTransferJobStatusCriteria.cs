namespace Fanex.ExternalAccount.DataAccess.Criteria.AutoTransfer
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DTO;

    public class GetAutoTransferJobStatusCriteria : INonQueryCommand
    {
        public GetAutoTransferJobStatusCriteria(SiteDB siteDB)
        {
            SiteDB = siteDB.IsSiteDB ? $"SiteDB{siteDB.SiteDBGroupID}_" : string.Empty;
            FromCustId = 0;
            ToCustId = 0;
            IsAllowAutoTransfer = true;
            ErrCode = 0;
            ErrMsg = string.Empty;
        }

        [SpParam(Ignored = true)]
        public string SiteDB { get; }

        public int FromCustId { get; }

        public int ToCustId { get; }

        public bool IsAllowAutoTransfer { get; }

        public bool IsAutoJobRunning { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public string GetSettingKey() => $"{SiteDB}Age_ClearBalanceCust_Job_StatusCheck" ;

        public bool IsValid() => true;
    }
}