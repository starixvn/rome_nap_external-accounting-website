namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System;
    using System.Data;
    using Extensions;
    using Newtonsoft.Json;

    public class OnlineInfo
    {
        public OnlineInfo(DataRow dataRow, Func<string, bool> canViewVNIP)
        {
            var onlineIp = dataRow.IP();
            IP = new ObjectInfo { Text = canViewVNIP(onlineIp) ? onlineIp : string.Empty };
            LoginTime = new ObjectInfo { Text = dataRow.ReloadTime() };
        }

        [JsonProperty(PropertyName = "iP")]
        public ObjectInfo IP { get; }

        public ObjectInfo LoginTime { get; }
    }
}