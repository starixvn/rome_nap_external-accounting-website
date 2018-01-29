namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Extensions;

    public class OnlineList
    {
        public OnlineList(DataRowCollection dataRows, Func<string, bool> canViewVNIP)
        {
            Onlines = new List<OnlineInfo>();
            foreach (DataRow dataRow in dataRows)
            {
                Onlines.Add(new OnlineInfo(dataRow, canViewVNIP));
            }
        }

        public List<OnlineInfo> Onlines { get; }
    }
}