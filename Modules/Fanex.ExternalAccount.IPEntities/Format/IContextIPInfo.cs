namespace Fanex.CustomerSetting.IP.Format
{
    public interface IContextIPInfo
    {
        int AdminId { get; set; }

        bool IsAdminCanViewVNIP { get; set; }

        string DefaultNoIPVN { get; set; }
    }
}
