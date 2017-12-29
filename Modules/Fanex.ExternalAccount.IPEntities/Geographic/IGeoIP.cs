namespace Fanex.CustomerSetting.IP.Geographic
{
    using DAL;

    public interface IGeoIP
    {
        IPTable LocationFromIP(string ip);
    }
}
