namespace Fanex.ExternalAccount.DTO
{
    public class SiteDB
    {
        public System.Int16 SiteDBGroupID { get; set; }

        public string LinkedServerName { get; set; }

        public string ClientAccessName { get; set; }

        public string AGIP { get; set; }

        public bool IsSiteDB => SiteDBGroupID != 0;
    }
}
