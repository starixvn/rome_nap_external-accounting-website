namespace Fanex.CustomerSetting.IP.Geographic
{
    using System.Configuration;
    using System.IO;
    using System.Xml.Linq;
    using DAL;

    internal class IbcIPService : GeoIP
    {
        private string _urlService = ConfigurationManager.AppSettings["ipservice"];

        public override IPTable LocationFromIP(string ip)
        {
            string responseContent = ReadUrl(string.Format("{0}?ip={1}", _urlService, ip));
            StringReader sr = new StringReader(responseContent);
            XElement respElement = XElement.Load(sr);

            return new IPTable
            {
                countrySHORT = (string)respElement.Element("CountryCode"),
                countryLONG = (string)respElement.Element("CountryName"),
                ipCity = (string)respElement.Element("City"),
                ipRegion = (string)respElement.Element("RegionName"),
                FlagPath = (string)respElement.Element("FlagPath")
            };
        }
    }
}
