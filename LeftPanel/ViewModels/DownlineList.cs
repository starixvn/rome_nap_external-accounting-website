using System.Collections.Generic;

namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    public class DownlineList : BasicInfo
    {
        public DownlineList()
        {
            this.Downlines = new List<ObjectInfo>();
            this.Pages = new List<ObjectInfo>();
            this.CurrentPage = 1;
        }

        public List<ObjectInfo> Downlines { get; set; }

        public List<ObjectInfo> Pages { get; set; }

        public int CurrentPage { get; set; }
    }
}
