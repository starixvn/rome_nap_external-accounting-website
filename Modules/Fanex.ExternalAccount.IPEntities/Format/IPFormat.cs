namespace Fanex.CustomerSetting.IP.Format
{
    using System;
    using System.Text.RegularExpressions;

    public class IPFormat
    {
        public IPFormat(string ip)
        {
            IP = ip;
        }

        protected IPFormat()
        {
        }

        public string IP { get; set; }

        protected virtual IContextIPInfo Context
        {
            get
            {
                return new ContextIPInfo
                {
                    AdminId = 0,
                    IsAdminCanViewVNIP = true,
                    DefaultNoIPVN = string.Empty
                };
            }
        }

        protected virtual string LinkFormat
        {
            get { return "<a href=\"javascript:OpenIPInfo('{0}');\" class=\"iplink\">{0}</a>"; }
        }

        protected virtual string LinkNoFormat
        {
            get { return "<span class='ipnolink'>{0}</span>"; }
        }

        public string Format()
        {
            if (string.IsNullOrEmpty(this.IP))
            {
                return string.Empty;
            }

            string result = string.Format(LinkFormat, this.IP);
            IContextIPInfo context = Context;
            bool isAdminLogin = context.AdminId > 0;
            bool isAdminCanViewVNIP = context.IsAdminCanViewVNIP;

            if (isAdminLogin)
            {
                bool isVNIP = Fanex.CustomerSetting.IP.Utils.IpUtils.IsVnIp(IP);
                if (isVNIP && !isAdminCanViewVNIP)
                {
                    result = string.Format(LinkNoFormat, context.DefaultNoIPVN);
                }
            }

            return result;
        }

        public string FormatAll(string content)
        {
            Regex regex = new Regex(Fanex.CustomerSetting.IP.Utils.IpUtils.IpPattern, RegexOptions.Compiled);
            return regex.Replace(
                content,
                match =>
                {
                    this.IP = match.Value;
                    return this.Format();
                });
        }
    }
}