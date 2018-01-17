namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System.Collections.Generic;

    public class DownlineList : BasicInfo
    {
        public DownlineList()
        {
            Downlines = new List<ObjectInfo>();
            Pages = new List<ObjectInfo>();
            CurrentPage = 1;
        }

        public List<ObjectInfo> Downlines { get; set; }

        public List<ObjectInfo> Pages { get; set; }

        public int CurrentPage { get; set; }
    }
}