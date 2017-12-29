namespace Fanex.CustomerSetting.IP.Geographic
{
    using System.Text.RegularExpressions;
    using DAL;

    public class IP2LocationService : GeoIP
    {
        // TODO: Should move to app.config for easy to change later
        private string _urlService = "http://www.ip2location.com/{0}";
        private string _templateReg =
        @"<span id=""dgLookup__ctl2_lblICountry"">([\w\s]+)</span>[\w\W\s]*<span id=""dgLookup__ctl2_lblICity"">" +
        @"([\w\-\s]+)</span>[\w\W\s]*<span id=""dgLookup__ctl2_lblITimeZone"">([\w\+\:\s]+)</span>";

        public override IPTable LocationFromIP(string ip)
        {
            string responseContent = ReadUrl(string.Format(_urlService, ip));
            Regex reg = new Regex(_templateReg, RegexOptions.Compiled);
            Match matchCountry = reg.Match(responseContent);
            string countryName = matchCountry.Groups[1].Value;
            string city = matchCountry.Groups[2].Value;
            string timeZone = matchCountry.Groups[3].Value;

            return new IPTable { countrySHORT = countryName, ipCity = city };
        }
    }
}
