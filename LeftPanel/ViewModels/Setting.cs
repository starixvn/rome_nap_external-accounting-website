namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System.Collections.Generic;

    public class Setting
    {
        public Setting(
            List<ObjectInfo> enableItems,
            List<ObjectInfo> syncItems,
            List<ObjectInfo> editItems,
            List<ObjectInfo> details)
        {
            EnableItems = enableItems;
            SyncItems = syncItems;
            EditItems = editItems;
            Details = details;
            MinMaxPts = new List<ObjectInfo>
            {
                new ObjectInfo {Key = "MinSB", Value = "Sportsbook"}
            };
            SuperPts = new List<ObjectInfo>();
            MasterPts = new List<ObjectInfo>();
        }

        public List<ObjectInfo> EnableItems { get; }

        public List<ObjectInfo> SyncItems { get; }

        public List<ObjectInfo> EditItems { get; }

        public List<ObjectInfo> Details { get; }

        public List<ObjectInfo> MinMaxPts { get; }

        public List<ObjectInfo> SuperPts { get; set; }

        public List<ObjectInfo> MasterPts { get; set; }
    }
}