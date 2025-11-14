namespace Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Fanex.ExternalAccount.Utilities;

    public class SuperWinLossDetailBySubGroupViewModel
    {
        public SuperWinLossDetailBySubGroupViewModel(
            int groupId,
            string groupName,
            IEnumerable<string> subGroupNames)
        {
            GroupId = groupId;
            GroupName = groupName;

            var subGroupList = subGroupNames.Select(name => BuildSelectListItem(name)).ToList();
            subGroupList.Insert(0, BuildSelectListItem(ViewLang.All, true));
            SubGroups = subGroupList;
        }

        public int GroupId { get; }

        public string GroupName { get; }

        public IEnumerable<SelectListItem> SubGroups { get; }

        private static SelectListItem BuildSelectListItem(string text, bool selected = false)
            => new SelectListItem { Value = text, Text = text, Selected = selected };
    }
}
