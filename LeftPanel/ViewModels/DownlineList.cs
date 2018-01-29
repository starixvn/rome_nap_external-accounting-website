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

        public ObjectInfo CustId { get; set; }

        public ObjectInfo UplineId { get; set; }

        public ObjectInfo RoleId { get; set; }

        public ObjectInfo UplineRoleId { get; set; }

        public List<ObjectInfo> Downlines { get; set; }

        public List<ObjectInfo> Pages { get; set; }

        public int CurrentPage { get; set; }

        internal static void AddPageItems(int totalPages, DownlineList model)
        {
            if (totalPages > 1)
            {
                for (var i = 1; i <= totalPages; i++)
                {
                    model.Pages.Add(new ObjectInfo { Value = i, Text = i.ToString() });
                }
            }
            else
            {
                model.Pages.Add(new ObjectInfo { Value = 1, Text = "1" });
            }
        }

        internal static void AddDownlines(DataTable downlines, DownlineList model)
        {
            if (downlines?.Rows == null)
            {
                return;
            }

            foreach (DataRow item in downlines.Rows)
            {
                model.Downlines.Add(new ObjectInfo { Text = item.Username() });
            }
        }
    }
}