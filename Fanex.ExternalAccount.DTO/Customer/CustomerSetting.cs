namespace Fanex.ExternalAccount.DTO.Customer
{
    public class CustomerSetting
    {
        public int CustId { get; set; }

        public int RoleId { get; set; }

        public int SuperRecommendId { get; set; }

        public int MasterRecommendId { get; set; }

        public int RecommendId { get; set; }

        public bool SyncCasino { get; set; }

        public bool SyncLiveCasino { get; set; }

        public bool AllowDoubleComm { get; set; }
    }
}