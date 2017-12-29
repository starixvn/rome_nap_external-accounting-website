namespace Fanex.CustomerSetting.IP.Geographic
{
    public class GeoIPService
    {
        private static IGeoIP _instance = null;

        private GeoIPService()
        {
        }

        public static IGeoIP GetInstance()
        {
            if (_instance == null)
            {
                _instance = new IbcIPService();
            }

            return _instance;
        }
    }
}
