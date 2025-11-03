namespace Fanex.ExternalAccount.DTO
{
    public class SiteDB
    {
        public short SiteDBGroupID { get; set; }

        public short CustDBGroupID { get; set; }

        public string LinkedServerName { get; set; }

        public string ClientAccessName { get; set; }

        public string AGIP { get; set; }

        public string AGIP_DBCUST { get; set; }

        public bool IsSiteDB => SiteDBGroupID != 0;
    }
}
