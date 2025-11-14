namespace Fanex.ExternalAccount.HeadlineBonus.ViewModels
{
    using System.Collections.Generic;
    using Fanex.ExternalAccount.HeadlineBonus.Constants;
    using Fanex.ExternalAccount.HeadlineBonus.Models;
    using Fanex.ExternalAccount.HeadlineBonus.Resources;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class BonusDetailViewModel
    {
        public BonusDetailViewModel(bool hasHostPermission)
        {
            HasHostPermission = hasHostPermission;
        }

        public string Note
            => string.Format(ViewLang.PendingBonusNote, MemberBonus.PendingBonusCancellableHours);

        public bool HasHostPermission { get; }

        public IEnumerable<SelectListItem> BonusStatus
            => new[]
            {
                GetSelectListItem(ViewLang.All, "-1", true),
                GetSelectListItem(BonusStatusIdConst.GetBonusStatus(BonusStatusIdConst.Pending), BonusStatusIdConst.Pending.ToString()),
                GetSelectListItem(BonusStatusIdConst.GetBonusStatus(BonusStatusIdConst.Cancelled), BonusStatusIdConst.Cancelled.ToString()),
                GetSelectListItem(BonusStatusIdConst.GetBonusStatus(BonusStatusIdConst.Completed), BonusStatusIdConst.Completed.ToString())
            };

        public IEnumerable<SelectListItem> MemberFeedback
            => new[]
            {
                GetSelectListItem(ViewLang.All, "-1", true),
                GetSelectListItem(ViewLang.NotReceived, "true"),
                GetSelectListItem(ViewLang.Received, "false")
            };

        private static SelectListItem GetSelectListItem(string text, string value, bool selected = false)
            => new SelectListItem { Text = text, Value = value, Selected = selected };
    }
}
