namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System.Collections.Generic;
    using System.Data;
    using Extensions;

    public class DownlineList
    {
        public DownlineList()
        {
            Downlines = new List<ObjectInfo>();
            Pages = new List<ObjectInfo>();
            CurrentPage = 1;
        }

        public DownlineList(DataSet dataSet, int pageIndex, int totalPages)
        {
            Downlines = new List<ObjectInfo>();
            Pages = new List<ObjectInfo>();

            if (dataSet == null || dataSet.Tables.Count <= 0)
            {
                return;
            }

            var downlines = dataSet.Tables[0];

            if (downlines?.Rows.Count > 0)
            {
                AddDownlines(downlines);
                CurrentPage = pageIndex;
                AddPageItems(totalPages);
            }
        }

        public ObjectInfo CustId { get; set; }

        public ObjectInfo UplineId { get; set; }

        public ObjectInfo RoleId { get; set; }

        public ObjectInfo UplineRoleId { get; set; }

        public List<ObjectInfo> Downlines { get; set; }

        public List<ObjectInfo> Pages { get; set; }

        public int CurrentPage { get; set; }

        internal void AddPageItems(int totalPages)
        {
            if (totalPages > 1)
            {
                for (var i = 1; i <= totalPages; i++)
                {
                    Pages.Add(new ObjectInfo { Value = i, Text = i.ToString() });
                }
            }
            else
            {
                Pages.Add(new ObjectInfo { Value = 1, Text = "1" });
            }
        }

        internal void AddDownlines(DataTable downlines)
        {
            if (downlines?.Rows == null)
            {
                return;
            }

            foreach (DataRow item in downlines.Rows)
            {
                Downlines.Add(new ObjectInfo { Text = item.Username() });
            }
        }
    }
}