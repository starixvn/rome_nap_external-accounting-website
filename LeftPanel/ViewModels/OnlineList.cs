namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System.Collections.Generic;

    public class OnlineList
    {
        public OnlineList()
        {
            Onlines = new List<OnlineInfo>();
        }

        public List<OnlineInfo> Onlines { get; set; }
    }
}