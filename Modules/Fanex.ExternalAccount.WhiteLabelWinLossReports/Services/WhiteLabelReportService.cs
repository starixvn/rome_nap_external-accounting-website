namespace Fanex.ExternalAccount.WhiteLabelWinLossReports.Services
{
    using System.Collections.Generic;
    using Fanex.CustomerSetting.Module.Core.Extensions;
    using Fanex.ExternalAccount.WhiteLabelWinLossReports.Criteria;
    using Fanex.ExternalAccount.WhiteLabelWinLossReports.Models;
    using Fanex.ExternalAccount.WhiteLabelWinLossReports.Requests;
    // using Fanex.NAP.Framework.Data;
    using Fanex.Data.Repository;
    
    public interface IWhiteLabelReportService
    {
        IEnumerable<WhiteLabelModel> GetWhiteLabels(string groupName);

        SettlementResult GetSettlement(WhiteLabelReportQuery message);

        DailyWinLossResult GetDailyWinLoss(WhiteLabelReportQuery message);
    }

    public class WhiteLabelReportService : IWhiteLabelReportService
    {
        private readonly IDynamicRepository repository;

        public WhiteLabelReportService(IDynamicRepository repository = null)
        {
            this.repository = repository ?? new DynamicRepository();
        }

        public IEnumerable<WhiteLabelModel> GetWhiteLabels(string groupName)
            => repository.Fetch<WhiteLabelModel>(new GetWhiteLabelListCriteria(groupName));

        public SettlementResult GetSettlement(WhiteLabelReportQuery message)
        {
            var settlements = repository.Fetch<Settlement>(new GetSettlementCriteria(
                message.GroupName,
                message.FromDate.FirstDayOfMonth(),
                message.ToDate.LastDayOfMonth(),
                message.WhiteLabels,
                message.Username));

            return new SettlementResult(settlements, AccumulateSettlementTotal(settlements));
        }

        private static Settlement AccumulateSettlementTotal(IEnumerable<Settlement> settlements)
        {
            var total = new Settlement();
            foreach (var item in settlements)
            {
                total += item;
            }

            return total;
        }

        public DailyWinLossResult GetDailyWinLoss(WhiteLabelReportQuery message)
        {
            var winloss = repository.Fetch<DailyWinLoss>(new GetDailyWinLossCriteria(
                message.GroupName,
                message.FromDate,
                message.ToDate,
                message.WhiteLabels,
                message.Username));

            return new DailyWinLossResult(winloss, AccumulateDailyWinLossTotal(winloss));
        }

        private static DailyWinLoss AccumulateDailyWinLossTotal(IEnumerable<DailyWinLoss> winloss)
        {
            var total = new DailyWinLoss();
            foreach (var item in winloss)
            {
                total += item;
            }

            return total;
        }
    }
}