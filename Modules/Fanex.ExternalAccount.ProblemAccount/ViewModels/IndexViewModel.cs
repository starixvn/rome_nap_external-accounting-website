namespace Fanex.ExternalAccount.ProblemAccount.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.ProblemAccount.Extensions;
    using Fanex.ExternalAccount.ProblemAccount.Resources;

    public class IndexViewModel
    {
        public IndexViewModel()
        {
            var today = DateTime.Today;
            DefaultFrom = today.AddDays(-13).ToDateString();
            DefaultTo = today.ToDateString();
        }

        public string DefaultFrom { get; }

        public string DefaultTo { get; }

        public IEnumerable<SelectListItem> ReviewStatus
            => new SelectListItem[]
            {
                new SelectListItem { Text = ViewLang.All, Value = "2", Selected = true },
                new SelectListItem { Text = ViewLang.Reviewed, Value = "1" },
                new SelectListItem { Text = ViewLang.NotReviewed, Value = "0" }
            };
    }
}
