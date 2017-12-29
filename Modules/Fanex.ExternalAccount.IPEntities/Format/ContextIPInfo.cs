namespace Fanex.CustomerSetting.IP.Format
{
    public class ContextIPInfo : IContextIPInfo
    {
        public int AdminId { get; set; }

        public bool IsAdminCanViewVNIP { get; set; }

        public string DefaultNoIPVN { get; set; }
    }
}
