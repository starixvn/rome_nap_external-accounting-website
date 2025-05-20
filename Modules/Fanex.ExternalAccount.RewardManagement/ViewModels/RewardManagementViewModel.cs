namespace Fanex.ExternalAccount.RewardManagement.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Fanex.ExternalAccount.Core.Shared.Extensions;
    using Fanex.ExternalAccount.RewardManagement.Models;

    public class RewardManagementViewModel
    {
        public RewardManagementViewModel(IEnumerable<Program> programs)
        {
            Programs = programs;
        }

        public string FormatedToday { get; } = DateTime.Today.ToShortDateWithSlash();

        public string FormatedMinDate { get; } = DateTime.Today.AddMonths(-3).FirstDayOfMonth().ToShortDateWithSlash();

        public IEnumerable<Program> Programs { get; }
    }
}