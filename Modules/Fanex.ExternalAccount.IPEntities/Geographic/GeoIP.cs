namespace Fanex.CustomerSetting.IP.Geographic
{
    using System.IO;
    using System.Net;
    using DAL;

    public abstract class GeoIP : IGeoIP
    {
        public static string ReadUrl(string url)
        {            
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public abstract IPTable LocationFromIP(string ip);
    }
}
