namespace Fanex.ExternalAccount.ProblemAccount.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Fanex.ExternalAccount.ProblemAccount.Constants;
    using Fanex.ExternalAccount.ProblemAccount.Extensions;
    using Fanex.ExternalAccount.ProblemAccount.Resources;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class IndexViewModel
    {
        private const int DefaultFromDays = -13;

        public IndexViewModel(bool hasHostPermission)
        {
            var today = DateTime.Today;
            DefaultFromDate = today.AddDays(DefaultFromDays).ToDateWithSlash();
            DefaultToDate = today.ToDateWithSlash();
            HasHostPermission = hasHostPermission;
        }

        public string DefaultFromDate { get; }

        public string DefaultToDate { get; }

        public IEnumerable<SelectListItem> ReviewStatus
            => new[]
            {
                new SelectListItem { Text = ViewLang.All, Value = ReviewStatusConst.All.ToString(), Selected = true },
                new SelectListItem { Text = ViewLang.Reviewed, Value = ReviewStatusConst.Reviewed.ToString() },
                new SelectListItem { Text = ViewLang.NotReviewed, Value = ReviewStatusConst.NotReviewed.ToString() }
            };

        public bool HasHostPermission { get; }
    }
}
