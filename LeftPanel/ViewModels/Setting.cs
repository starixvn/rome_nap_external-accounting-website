namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System.Collections.Generic;

    public class Setting
    {
        public Setting()
        {
            EnableItems = new List<ObjectInfo>();
            SyncItems = new List<ObjectInfo>();
            EditItems = new List<ObjectInfo>();
            Details = new List<ObjectInfo>();
            MinMaxPts = new List<ObjectInfo>();
            SuperPts = new List<ObjectInfo>();
            MasterPts = new List<ObjectInfo>();
        }

        public List<ObjectInfo> EnableItems { get; set; }

        public List<ObjectInfo> SyncItems { get; set; }

        public List<ObjectInfo> EditItems { get; set; }

        public List<ObjectInfo> Details { get; set; }

        public List<ObjectInfo> MinMaxPts { get; set; }

        public List<ObjectInfo> SuperPts { get; set; }

        public List<ObjectInfo> MasterPts { get; set; }
    }
}