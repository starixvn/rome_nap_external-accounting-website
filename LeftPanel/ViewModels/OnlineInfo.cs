namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using Newtonsoft.Json;

    public class OnlineInfo
    {
        public OnlineInfo(ObjectInfo ip, ObjectInfo loginTime)
        {
            IP = ip;
            LoginTime = loginTime;
        }

        [JsonProperty(PropertyName = "iP")]
        public ObjectInfo IP { get; }

        public ObjectInfo LoginTime { get; }
    }
}
