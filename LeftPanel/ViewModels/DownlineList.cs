namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System.Collections.Generic;

    public class DownlineList
    {
        public DownlineList()
        {
            Downlines = new List<ObjectInfo>();
            Pages = new List<ObjectInfo>();
            CurrentPage = 1;
        }

        public ObjectInfo CustId { get; set; }

        public ObjectInfo UplineId { get; set; }

        public ObjectInfo RoleId { get; set; }

        public ObjectInfo UplineRoleId { get; set; }

        public List<ObjectInfo> Downlines { get; set; }

        public List<ObjectInfo> Pages { get; set; }

        public int CurrentPage { get; set; }
    }
}