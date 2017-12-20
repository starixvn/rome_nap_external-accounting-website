namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    public class OnlineInfo
    {
        public OnlineInfo(ObjectInfo ip, ObjectInfo loginTime)
        {
            IP = ip;
            LoginTime = loginTime;
        }

        public ObjectInfo IP { get; }

        public ObjectInfo LoginTime { get; }
    }
}
