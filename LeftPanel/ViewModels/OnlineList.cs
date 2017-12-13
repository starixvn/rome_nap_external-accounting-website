using System.Collections.Generic;

namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    public class OnlineList
    {
        public OnlineList()
        {
            this.Onlines = new List<OnlineInfo>();
        }

        public List<OnlineInfo> Onlines { get; set; }
    }
}
