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

        public OnlineList(DataSet dataSet, Func<string, bool> canViewVNIP) : this()
        {
            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                Onlines.Add(new OnlineInfo(dataRow, canViewVNIP));
            }
        }

        public List<OnlineInfo> Onlines { get; }
    }
}