namespace Fanex.ExternalAccount.RewardManagement.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Fanex.CustomerSetting.Module.Core.Extensions;
    using Fanex.ExternalAccount.RewardManagement.Models;

    public class RewardManagementViewModel
    {
        public RewardManagementViewModel(IEnumerable<Program> programs)
        {
            Programs = programs;
        }

        public string FormatedToday { get; } = DateTime.Today.ToShortFormatedDateWithSlash();

        public string FormatedMinDate { get; } = DateTime.Today.AddMonths(-3).FirstDayOfMonth().ToShortFormatedDateWithSlash();

        public IEnumerable<Program> Programs { get; }
    }
}