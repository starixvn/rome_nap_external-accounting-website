namespace Fanex.CustomerSetting.IP.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Caching;
    using DAL;

    public static class IpUtils
    {
        public const string IpLinkFormat = "<a class=\"iplink\" href=\"javascript:OpenIPInfo('{0}');\">{0}</a>";
        public const string IpPattern = @"(?<First>2[0-4]\d|25[0-5]|[01]?\d\d?)\.(?<Second>2[0-4]\d|25[0-5]|[01]?\d\d?)\." + 
            @"(?<Third>2[0-4]\d|25[0-5]|[01]?\d\d?)\.(?<Fourth>2[0-4]\d|25[0-5]|[01]?\d\d?)";

        public const string IpVnKey = "Fanex.CustomerSetting.IP.IpVnKey";

        private static List<IPVN> ipVNList = null;

        public static double ConvertIpToNumber(string ip)
        {
            double num = 0.0;
            if (IsValidIp(ip))
            {
                string[] strArray = ip.Split(new char[] { '.' });
                num = (((double.Parse(strArray[0]) * 16777216.0) + (double.Parse(strArray[1]) * 65536.0)) + 
                    (double.Parse(strArray[2]) * 256.0)) + double.Parse(strArray[3]);
            }

            return num;
        }
        
        public static string FormatIp(string ip)
        {
            return string.Format("<a class=\"iplink\" href=\"javascript:OpenIPInfo('{0}');\">{0}</a>", ip);
        }

        public static List<IPVN> GetAllVnIps()
        {
            ICache cache = CacheService.CreateInstance(CacheType.MemoryCache);
            List<IPVN> objA = cache.Get<List<IPVN>>("Fanex.CustomerSetting.IP.IpVnKey");
            if (object.ReferenceEquals(objA, null))
            {
                using (IPEntities entities = new IPEntities(Utility.GetEFConnectionString()))
                {
                    objA = (from s in entities.GetIPVN() select s).ToList<IPVN>();
                    if (objA.Count > 0)
                    {
                        cache.Set<List<IPVN>>("Fanex.CustomerSetting.IP.IpVnKey", objA, TimeSpan.FromHours(1.0));
                    }
                }
            }

            ipVNList = objA;
            return objA;
        }
        
        public static bool IsValidIp(string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                return false;
            }

            Regex regex = new Regex(@"(?<First>2[0-4]\d|25[0-5]|[01]?\d\d?)\.(?<Second>2[0-4]\d|25[0-5]|[01]?\d\d?)\." + 
                @"(?<Third>2[0-4]\d|25[0-5]|[01]?\d\d?)\.(?<Fourth>2[0-4]\d|25[0-5]|[01]?\d\d?)");
            return regex.IsMatch(ip, 0);
        }

        public static bool IsVnIp(string ip)
        {
            Predicate<IPVN> match = null;
            double ipNumber;
            if (!string.IsNullOrEmpty(ip))
            {
                ipNumber = ConvertIpToNumber(ip);
                List<IPVN> list = ipVNList ?? GetAllVnIps();
                if (list != null)
                {
                    if (match == null)
                    {
                        match = ipvn => (ipvn.IPFrom <= ipNumber) && (ipNumber <= ipvn.IPTo);
                    }

                    return list.Exists(match);
                }
            }

            return false;
        }

        public static IPTable LocationFromIP(string ip)
        {
            using (IPEntities entities = new IPEntities(Utility.GetEFConnectionString()))
            {
                List<IPTable> list = entities.GetIPInfo_Full(ip).ToList<IPTable>();
                if ((list != null) && (list.Count > 0))
                {
                    return list[0];
                }
            }

            return new IPTable();
        }
    }
}
