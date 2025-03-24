namespace Fanex.ExternalAccount.Core.Criteria
{
    using Fanex.Data.Repository;

    public class CheckCustomerIsSpecialSiteCommand : INonQueryCommand
    {
        public string UserName { get; set; }

        public byte? CheckCustId { get; set; }

        public bool IsSpecialSite { get; set; }

        public bool IsSpecialHeadline { get; set; }

        public string Site { get; set; }

        public string GetSettingKey() => "Acc_CustomerIsSpecialSite";

        public bool IsValid() => true;
    }
}
