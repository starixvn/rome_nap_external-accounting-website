namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public class OnlineList
    {
        public OnlineList()
        {
            Onlines = new List<OnlineInfo>();
        }

        public OnlineList(DataRowCollection dataRows, Func<string, bool> canViewVNIP) : this()
        {
            foreach (DataRow dataRow in dataRows)
            {
                Onlines.Add(new OnlineInfo(dataRow, canViewVNIP));
            }
        }

        public List<OnlineInfo> Onlines { get; }
    }
}